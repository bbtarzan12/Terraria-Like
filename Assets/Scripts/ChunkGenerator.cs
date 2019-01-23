using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int chunkSize;
    [SerializeField] int chunkIteration = 3;
    [SerializeField] Camera playerCamera;

    [SerializeField] ColorTilebaseDictionary tileDictionary;
    TileBase[] tiles;
    [SerializeField] NoiseGraph graph;
    Texture2D mapTexture;
    Color[] mapArray;
    Tilemap tilemap;

    JobHandle jobHandle;

    Transform playerTransform;
    Vector3 playerPosition => playerTransform.position;
    Vector2Int lastChunkPosition = new Vector2Int(-1, -1);
    Vector2Int chunkPosition => new Vector2Int( Mathf.RoundToInt(playerPosition.x / chunkSize.x), Mathf.RoundToInt(playerPosition.y / chunkSize.y));
    Vector2Int mapSize;
    int arraySize;
    TileBase[] tileBases;
    Vector3Int[] positions;
    Queue<TileProcessingQueueData> tileProcessingQueue;
    List<Vector2Int> visitedChunkPosition;

    NativeArray<Color> colors;
    NativeArray<Color> oreColors;

    [SerializeField] int numProcessTileJobSegment;
    [SerializeField] int numProcessTileQueueSegment;

    struct TileProcessingQueueData
    {
        public int length;
        public TileBase[] tiles;
        public Vector3Int[] positions;
    }
    
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        playerTransform = playerCamera.transform;
        tileProcessingQueue = new Queue<TileProcessingQueueData>();
        
        tileDictionary = new ColorTilebaseDictionary();
        RuleTile[] oreTiles = Resources.LoadAll<RuleTile>("Ores");
        
        tileDictionary.Add(Color.black, null);
        foreach (var tile in oreTiles)
        {
            tileDictionary.Add(new Color(tile.color.r, tile.color.g, tile.color.b, 1), tile);
        }

        tiles = tileDictionary.Values.ToArray();
        visitedChunkPosition = new List<Vector2Int>();
    }

    void Start()
    {
        mapTexture = graph.GetTexture();
        mapArray = mapTexture.GetPixels();
        mapSize = graph.mapSize;
        arraySize = chunkIteration * chunkIteration * chunkSize.x * chunkSize.y;
        tileBases = new TileBase[arraySize];
        positions = new Vector3Int[arraySize];
        
        colors = new NativeArray<Color>(mapArray, Allocator.Persistent);
        oreColors = new NativeArray<Color>(tileDictionary.Keys.ToArray(), Allocator.Persistent);
        
        StartCoroutine(nameof(ProcessTileJob));
        StartCoroutine(nameof(ProcessTileQueue));
    }

    void OnDestroy()
    {
        colors.Dispose();
        oreColors.Dispose();
    }

    struct TileBlockJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<Color> colors;
        [WriteOnly] public NativeArray<int> tiles;
        [WriteOnly] public NativeArray<int2> pos;

        [ReadOnly] public int chunkIteration;
        [ReadOnly] public NativeArray<Color> oreColors;
        [ReadOnly] public int2 mapSize;
        [ReadOnly] public int2 chunkSize;
        [ReadOnly] public int2 chunkPos;

        public void Execute(int index)
        {
            int2 indexCoord = new int2(index % (chunkIteration * chunkSize.x), index / (chunkIteration * chunkSize.x));
            indexCoord.x -= chunkIteration * chunkSize.x / 2;
            indexCoord.y -= chunkIteration * chunkSize.y / 2;
            
            int2 coord = new int2(chunkSize.x * chunkPos.x + indexCoord.x, chunkSize.y * chunkPos.y + indexCoord.y);
                        
            if (mapSize.x < coord.x || mapSize.y < coord.y || coord.x < 0 || coord.y < 0)
                return;
            
            int coordIndex = coord.x + coord.y * mapSize.x; 

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
            pos[index] = coord;
        }
    }

    IEnumerator ProcessTileJob()
    {
        int[] indexesBuffer = new int[arraySize];
        int2[] positionBuffer = new int2[arraySize];
        
        while (true)
        {
            yield return null;
            if (chunkPosition == lastChunkPosition)
                continue;

            if (visitedChunkPosition.Contains(chunkPosition))
                continue;

            lastChunkPosition = chunkPosition;    
            
            NativeArray<int> tileIndexes = new NativeArray<int>(arraySize, Allocator.TempJob);
            NativeArray<int2> tilePositions = new NativeArray<int2>(arraySize, Allocator.TempJob);
                
            var tileBlockJob = new TileBlockJob()
            {
                colors = colors,
                tiles = tileIndexes,
                pos = tilePositions,
                mapSize = new int2(mapSize.x, mapSize.y),
                chunkIteration = chunkIteration,
                chunkSize = new int2(chunkSize.x, chunkSize.y),
                chunkPos = new int2(lastChunkPosition.x, lastChunkPosition.y),
                oreColors = oreColors
            };
   
            jobHandle = tileBlockJob.Schedule(arraySize, 32);

            jobHandle.Complete();
            
            visitedChunkPosition.Add(chunkPosition);
            
            tileIndexes.CopyTo(indexesBuffer);
            tilePositions.CopyTo(positionBuffer);
            
            tileIndexes.Dispose();
            tilePositions.Dispose();
            
            int segment = arraySize / numProcessTileJobSegment;
            for (int i = 0; i < arraySize; i++)
            {
                if (i % segment == 0)
                    yield return null;
                tileBases[i] = tiles[indexesBuffer[i]];
                positions[i] = new Vector3Int(positionBuffer[i].x, positionBuffer[i].y, 0);
            }
            
            tileProcessingQueue.Enqueue(new TileProcessingQueueData {length = arraySize, tiles = tileBases, positions = positions});
        }
    }

    IEnumerator ProcessTileQueue()
    {
        Vector3Int[] positionSegmentBuffer = new Vector3Int[arraySize / numProcessTileQueueSegment];
        TileBase[] tileBasesSegementBuffer = new TileBase[arraySize / numProcessTileQueueSegment];
        
        while (true)
        {
            while(tileProcessingQueue.Count == 0)
                yield return null;

            TileProcessingQueueData data = tileProcessingQueue.Dequeue();

            int segment = data.length / numProcessTileQueueSegment;
            for (int i = 0; i < numProcessTileQueueSegment; i++)
            {
                ArraySegment<Vector3Int> positionSegment = new ArraySegment<Vector3Int>(data.positions, i * segment, segment);
                ArraySegment<TileBase> tilesSegment = new ArraySegment<TileBase>(data.tiles, i * segment, segment);
                Array.Copy(data.positions, positionSegment.Offset, positionSegmentBuffer, 0, positionSegment.Count);
                Array.Copy(data.tiles, tilesSegment.Offset, tileBasesSegementBuffer, 0, tilesSegment.Count);
                tilemap.SetTiles(positionSegmentBuffer, tileBasesSegementBuffer);
                yield return null;
            }
            
        }
    }
}