using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int chunkSize;
    [SerializeField] Camera playerCamera;

    [SerializeField] TileBase redTile;
    [SerializeField] TileBase greenTile;
    [SerializeField] TileBase blueTile;
    [SerializeField] TileBase yellowTile;
    [SerializeField] TileBase whiteTile;
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
    Queue<TileQueueData> tileQueue;

    struct TileQueueData
    {
        public int length;
        public TileBase[] tiles;
        public Vector3Int[] positions;
    }
    
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        playerTransform = playerCamera.transform;
        tileQueue = new Queue<TileQueueData>();
    }

    void Start()
    {
        mapTexture = graph.GetTexture();
        mapArray = mapTexture.GetPixels();
        mapSize = graph.mapSize;
        arraySize = 9 * chunkSize.x * chunkSize.y;
        tileBases = new TileBase[arraySize];
        positions = new Vector3Int[arraySize];

        ProcessTileQueue();
    }

    void LateUpdate()
    {
        UpdateTileQueue();
    }

    struct TileBlockJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<Color> colors;
        [WriteOnly] public NativeArray<int> tiles;
        [WriteOnly] public NativeArray<int2> pos;

        [ReadOnly] public int colorsLength;
        [ReadOnly] public int2 mapSize;
        [ReadOnly] public int2 chunkSize;
        [ReadOnly] public int2 chunkPos;

        public void Execute(int index)
        {
            int2 indexCoord = new int2(index % (3 * chunkSize.x), index / (3 * chunkSize.y));
            indexCoord.x -= 3 * chunkSize.x / 2;
            indexCoord.y -= 3 * chunkSize.y / 2;
            
            int2 coord = new int2(chunkSize.x * chunkPos.x + indexCoord.x, chunkSize.y * chunkPos.y + indexCoord.y);
                        
            if (mapSize.x < coord.x || mapSize.y < coord.y || coord.x < 0 || coord.y < 0)
                return;
            
            int coordIndex = coord.x + coord.y * mapSize.x; 

            if (colorsLength <= coordIndex)
                return;
                
            Color color = colors[coordIndex];

            const float threshold = 0.3f;

            if (color.r > threshold && color.g > threshold && color.b > threshold)
                tiles[index] = 1;
            else if (color.r > threshold && color.g > threshold)
                tiles[index] = 5;
            else if (color.r > threshold)
                tiles[index] = 2;
            else if (color.g > threshold)
                tiles[index] = 3;
            else if (color.b > threshold)
                tiles[index] = 4;
            else
                tiles[index] = 0;
            
            pos[index] = coord;
        }
    }


    void UpdateTileQueue()
    {
        if (chunkPosition == lastChunkPosition)
            return;
        
        lastChunkPosition = chunkPosition;
        
        NativeArray<Color> colors = new NativeArray<Color>(mapArray, Allocator.TempJob);
        NativeArray<int> tileIndexes = new NativeArray<int>(arraySize, Allocator.TempJob);
        NativeArray<int2> tilePositions = new NativeArray<int2>(arraySize, Allocator.TempJob);
        var tileBlockJob = new TileBlockJob()
        {
            colors = colors,
            tiles = tileIndexes,
            pos = tilePositions,
            mapSize = new int2(mapSize.x, mapSize.y),
            chunkSize = new int2(chunkSize.x, chunkSize.y),
            chunkPos = new int2(lastChunkPosition.x, lastChunkPosition.y),
            colorsLength = colors.Length
        };

        jobHandle = tileBlockJob.Schedule(arraySize, 64);
        jobHandle.Complete();
        
        for (int i = 0; i < arraySize; i++)
        {
            TileBase tile;
            switch (tileIndexes[i])
            {
                case 0: tile = null;
                    break;
                case 1: tile = whiteTile;
                    break;
                case 2: tile = redTile;
                    break;
                case 3: tile = greenTile;
                    break;
                case 4: tile = blueTile;
                    break;
                case 5: tile = yellowTile;
                    break;
                default: tile = null;
                    break;
            }
            
            tileBases[i] = tile;
            positions[i] = new Vector3Int(tilePositions[i].x, tilePositions[i].y, 0);
        }

        tileQueue.Enqueue(new TileQueueData {length = arraySize, tiles = tileBases, positions = positions});
        colors.Dispose();
        tileIndexes.Dispose();
        tilePositions.Dispose();
    }

    async void ProcessTileQueue()
    {
        while (true)
        {
            while(tileQueue.Count == 0)
                await Task.Yield();

            TileQueueData data = tileQueue.Dequeue();

            int segment = data.length / 50;
            for (int i = 0; i < 50; i++)
            {
                ArraySegment<Vector3Int> positionSegment = new ArraySegment<Vector3Int>(data.positions, i * segment, segment);
                ArraySegment<TileBase> tilesSegment = new ArraySegment<TileBase>(data.tiles, i * segment, segment);
                tilemap.SetTiles(positionSegment.ToArray(), tilesSegment.ToArray());
                await Task.Yield();
            }
        }
    }
}