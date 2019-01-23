using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class ChunkUpdator : MonoBehaviour
{
    public enum TileType { Ores, Walls }
    
    [SerializeField] Transform target;
    [SerializeField] Vector2Int chunkSize = new Vector2Int(16, 16);
    [SerializeField] int chunkIteration = 5;
    [SerializeField] NoiseGraph graph;
    [SerializeField] TileType tileType;

    Tilemap tilemap;
    
    Vector2Int lastChunkPosition = new Vector2Int(int.MinValue, int.MaxValue);
    Vector2Int chunkPosition;
    
    ColorTilebaseDictionary tileDictionary;
    RuleTile[] tiles;
    Color[] mapColorArray;
    int arraySize;
    Vector2Int mapSize;
    
    TileBase[] tileBases;
    Vector3Int[] positions;
    List<TileChunk> visitedChunk;

    JobHandle jobHandle;
    NativeArray<Color> nativeMapColorArray;
    NativeArray<Color> nativeTileColorArray;
    
    int[] indexesBuffer;
    int2[] positionBuffer;

    Queue<TileChunk> drawChunkQueue;
    Queue<TileChunk> eraseChunkQueue;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        
        tileDictionary = new ColorTilebaseDictionary();
        RuleTile[] oreTiles = Resources.LoadAll<RuleTile>(tileType.ToString());

        tileDictionary.Add(Color.black, null);
        foreach (var tile in oreTiles)
        {
            tileDictionary.Add(new Color(tile.color.r, tile.color.g, tile.color.b, 1), tile);
        }

        tiles = tileDictionary.Values.ToArray();

        arraySize = chunkSize.x * chunkSize.y;
        
        tileBases = new TileBase[arraySize];
        positions = new Vector3Int[arraySize];
        visitedChunk = new List<TileChunk>();

        indexesBuffer = new int[arraySize];
        positionBuffer = new int2[arraySize];
        
        drawChunkQueue = new Queue<TileChunk>();
        eraseChunkQueue = new Queue<TileChunk>();
    }

    void Start()
    {
        Texture2D mapTexture = graph.GetTexture();
        mapColorArray = mapTexture.GetPixels();
        mapSize = graph.mapSize;
        
        nativeMapColorArray = new NativeArray<Color>(mapColorArray, Allocator.Persistent);
        nativeTileColorArray = new NativeArray<Color>(tileDictionary.Keys.ToArray(), Allocator.Persistent);

        StartCoroutine(nameof(ProcessDrawQueue));
        StartCoroutine(nameof(ProcessEraseQueue));
    }   

    void Update()
    {
        OnChunkUpdate();
    }

    void OnDestroy()
    {
        nativeMapColorArray.Dispose();
        nativeTileColorArray.Dispose();
    }
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        for (int x = chunkPosition.x - chunkIteration; x <= chunkPosition.x + chunkIteration; x++)
        {
            for (int y = chunkPosition.y - chunkIteration; y <= chunkPosition.y + chunkIteration; y++)
            {
                Vector2Int currentChunkPosition = new Vector2Int(x, y) * chunkSize;
                BoundsInt currentBound = new BoundsInt(new Vector3Int(currentChunkPosition.x, currentChunkPosition.y, 0) , new Vector3Int(chunkSize.x, chunkSize.y, 1));
                Gizmos.DrawWireCube(currentBound.center, currentBound.size);
            }
        }
    }

    [BurstCompile]
    struct ChunkBlockJob : IJobParallelFor
    {
        [WriteOnly] public NativeArray<int> tiles;
        [WriteOnly] public NativeArray<int2> pos;
        
        [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<Color> colors;
        [ReadOnly] public int2 mapSize;
        [ReadOnly] public BoundsInt bound;
        [ReadOnly] public NativeArray<Color> oreColors;
        
        public void Execute(int index)
        {
            int2 indexCoord = new int2(index % bound.size.x, index / bound.size.x);
            int2 worldCoord = new int2(indexCoord.x + bound.position.x, indexCoord.y + bound.position.y);
            
            if (mapSize.x < worldCoord.x || mapSize.y < worldCoord.y || worldCoord.x < 0 || worldCoord.y < 0)
                return;
            
            int coordIndex = worldCoord.x + worldCoord.y * mapSize.x;


            if (colors.Length <= coordIndex)
                return;
            
            Color color = colors[coordIndex];
            int colorIndex = 0;
            
            for (int i = 0; i < oreColors.Length; i++)
            {
                if(color != oreColors[i])
                    continue;

                colorIndex = i;
                break;
            }
            
            tiles[index] = colorIndex;
            pos[index] = worldCoord;
        }
    }

    void OnChunkUpdate()
    {
        Vector3 targetPosition = target.position;
        chunkPosition = new Vector2Int( Mathf.FloorToInt(targetPosition.x / chunkSize.x), Mathf.FloorToInt((targetPosition.y / chunkSize.y)));

        if (chunkPosition == lastChunkPosition)
            return;

        lastChunkPosition = chunkPosition;

        StartCoroutine(nameof(ChunkUpdateSequence));
    }

    IEnumerator ChunkUpdateSequence()
    {
        yield return StartCoroutine(nameof(GenerateDrawTileChunk));
        yield return StartCoroutine(nameof(GenerateEraseTileChunk));
    }

    IEnumerator GenerateDrawTileChunk()
    {
         for (int x = chunkPosition.x - chunkIteration; x <= chunkPosition.x + chunkIteration; x++)
        {
            for (int y = chunkPosition.y - chunkIteration; y <= chunkPosition.y + chunkIteration; y++)
            {
                Vector2Int currentChunkPosition = new Vector2Int(x, y) * chunkSize;
                Vector3Int currentBoundPosition = new Vector3Int(currentChunkPosition.x, currentChunkPosition.y, 0);

                if (visitedChunk.Exists(c => c.bound.position == currentBoundPosition))
                    continue;
                
                NativeArray<int> tileIndexes = new NativeArray<int>(arraySize, Allocator.TempJob);
                NativeArray<int2> tilePositions = new NativeArray<int2>(arraySize, Allocator.TempJob);
                    
                BoundsInt currentBound = new BoundsInt(currentBoundPosition , new Vector3Int(chunkSize.x, chunkSize.y, 1));

                var chunkBlockJob = new ChunkBlockJob
                {
                    colors = nativeMapColorArray,
                    tiles = tileIndexes,
                    pos = tilePositions,
                    bound = currentBound,
                    mapSize = new int2(mapSize.x, mapSize.y),
                    oreColors = nativeTileColorArray
                };
                
                jobHandle = chunkBlockJob.Schedule(arraySize, 32);
                jobHandle.Complete();
                
                tileIndexes.CopyTo(indexesBuffer);
                tilePositions.CopyTo(positionBuffer);
                    
                tileIndexes.Dispose();
                tilePositions.Dispose();
                
                
                for (int i = 0; i < arraySize; i++)
                {
                    tileBases[i] = tiles[indexesBuffer[i]];
                    positions[i] = new Vector3Int(positionBuffer[i].x, positionBuffer[i].y, 0);
                }

                TileChunk tileChunk = new TileChunk(currentBound, tileBases, tilemap);
                tileChunk.Draw();
                    
                drawChunkQueue.Enqueue(tileChunk);
                visitedChunk.Add(tileChunk);
                yield return null;
            }
        }
    }

    IEnumerator GenerateEraseTileChunk()
    {
        BoundsInt bigChunkBound = new BoundsInt((chunkPosition.x - chunkIteration) * chunkSize.x, (chunkPosition.y - chunkIteration) * chunkSize.y, 0, chunkSize.x * chunkIteration * 2 + 1, chunkSize.y * chunkIteration * 2 + 1, 1);

        for (int i = visitedChunk.Count - 1; i >= 0; i--)
        {
            var chunk = visitedChunk[i];
            if (bigChunkBound.Contains(chunk.bound.position))
                continue;

            eraseChunkQueue.Enqueue(chunk);
            visitedChunk.RemoveAt(i);
            yield return null;
        }
    }

    IEnumerator ProcessDrawQueue()
    {
        while (true)
        {
            while (drawChunkQueue.Count == 0)
                yield return null;

            TileChunk chunk = drawChunkQueue.Dequeue();
            
            chunk.Draw();
            yield return null;
        }
    }
    
    IEnumerator ProcessEraseQueue()
    {
        while (true)
        {
            while (eraseChunkQueue.Count == 0)
                yield return null;

            TileChunk chunk = eraseChunkQueue.Dequeue();
            
            chunk.Erase();
            yield return null;
        }
    }

}