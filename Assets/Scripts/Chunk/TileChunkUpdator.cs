using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChunkUpdator : MonoBehaviour
{
    [SerializeField] Vector2Int chunkSize;
    [SerializeField] int chunkIteration;
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
    Vector2Int chunkPosition => new Vector2Int( Mathf.FloorToInt(playerPosition.x / chunkSize.x), Mathf.FloorToInt((playerPosition.y / chunkSize.y)));
    Vector2Int mapSize;
    int arraySize;
    TileBase[] tileBases;
    Vector3Int[] positions;
    List<TileChunk> visitedChunk;

    NativeArray<Color> colors;
    NativeArray<Color> oreColors;
    
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        playerTransform = playerCamera.transform;
        
        tileDictionary = new ColorTilebaseDictionary();
        RuleTile[] oreTiles = Resources.LoadAll<RuleTile>("Ores");
        
        tileDictionary.Add(Color.black, null);
        foreach (var tile in oreTiles)
        {
            tileDictionary.Add(new Color(tile.color.r, tile.color.g, tile.color.b, 1), tile);
        }

        tiles = tileDictionary.Values.ToArray();
        visitedChunk = new List<TileChunk>();
    }

    void Start()
    {
        mapTexture = graph.GetTexture();
        mapArray = mapTexture.GetPixels();
        mapSize = graph.mapSize;
        arraySize = chunkSize.x * chunkSize.y;
        tileBases = new TileBase[arraySize];
        positions = new Vector3Int[arraySize];
        
        colors = new NativeArray<Color>(mapArray, Allocator.Persistent);
        oreColors = new NativeArray<Color>(tileDictionary.Keys.ToArray(), Allocator.Persistent);
        
        StartCoroutine(nameof(TileChunkSystem));
    }

    void OnDestroy()
    {
        colors.Dispose();
        oreColors.Dispose();
        StopAllCoroutines();
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
    
    IEnumerator TileChunkSystem()
    {
        int[] indexesBuffer = new int[arraySize];
        int2[] positionBuffer = new int2[arraySize];
        
        while (true)
        {
            yield return null;
            
            if (chunkPosition == lastChunkPosition)
                continue;
            
            lastChunkPosition = chunkPosition;

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
                        colors = colors,
                        tiles = tileIndexes,
                        pos = tilePositions,
                        bound = currentBound,
                        mapSize = new int2(mapSize.x, mapSize.y),
                        oreColors = oreColors
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
                    
                    visitedChunk.Add(tileChunk);
                    //tilemap.SetTilesBlock(currentBound, tileBases);
                    yield return null;
                }
            }
            
            BoundsInt bigChunkBound = new BoundsInt((chunkPosition.x - chunkIteration) * chunkSize.x, (chunkPosition.y - chunkIteration) * chunkSize.y, 0, chunkSize.x * chunkIteration * 2 + 1, chunkSize.y * chunkIteration * 2 + 1, 1);


            for (int i = visitedChunk.Count - 1; i >= 0; i--)
            {
                var chunk = visitedChunk[i];
                if (bigChunkBound.Contains(chunk.bound.position))
                    continue;

                chunk.Erase();
                visitedChunk.RemoveAt(i);
            }
            
        }
    }
}