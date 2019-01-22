using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
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
    Queue<JobDataProcessingQueueData> jobDataProcessingQueue;
    List<Vector2Int> visitedChunkPosition;

    struct TileProcessingQueueData
    {
        public int length;
        public TileBase[] tiles;
        public Vector3Int[] positions;
    }

    struct JobDataProcessingQueueData
    {
        public int length;
        public int[] indexes;
        public int2[] positions;
    }
    
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        playerTransform = playerCamera.transform;
        tileProcessingQueue = new Queue<TileProcessingQueueData>();
        jobDataProcessingQueue = new Queue<JobDataProcessingQueueData>();
        
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

        ProcessTileJob();
        ProcessTileQueue();
        ProcessJobDataQueue();
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
            int2 indexCoord = new int2(index % (chunkIteration * chunkSize.x), index / (chunkIteration * chunkSize.y));
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


    async void UpdateTileQueue()
    {
        if (chunkPosition == lastChunkPosition)
            return;

        if (visitedChunkPosition.Contains(chunkPosition))
            return;
        
        lastChunkPosition = chunkPosition;
        
        NativeArray<Color> colors = new NativeArray<Color>(mapArray, Allocator.TempJob);
        NativeArray<int> tileIndexes = new NativeArray<int>(arraySize, Allocator.TempJob);
        NativeArray<int2> tilePositions = new NativeArray<int2>(arraySize, Allocator.TempJob);
        NativeArray<Color> oreColors = new NativeArray<Color>(tileDictionary.Keys.ToArray(), Allocator.TempJob);
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
        
        if (!jobHandle.IsCompleted)
            Task.Yield();
        
        jobHandle.Complete();
                
        visitedChunkPosition.Add(chunkPosition);

        var jobProcessingData = new JobDataProcessingQueueData();
        jobProcessingData.indexes = new int[tileIndexes.Length];
        jobProcessingData.positions = new int2[tilePositions.Length];
        
        jobProcessingData.length = arraySize;
        tileIndexes.CopyTo(jobProcessingData.indexes);
        tilePositions.CopyTo(jobProcessingData.positions);
        
        jobDataProcessingQueue.Enqueue(jobProcessingData);
        
        colors.Dispose();
        tileIndexes.Dispose();
        tilePositions.Dispose();
        oreColors.Dispose();
    }

    async void ProcessTileJob()
    {
        while (true)
        {
            UpdateTileQueue();
            await Task.Yield();
        }
    }

    async void ProcessJobDataQueue()
    {
        while (true)
        {
            while (jobDataProcessingQueue.Count == 0)
                await Task.Yield();

            JobDataProcessingQueueData data = jobDataProcessingQueue.Dequeue();

            int segment = data.length / 64;
            for (int i = 0; i < arraySize; i++)
            {
                if (i % segment == 0)
                    await Task.Yield();
                tileBases[i] = tiles[data.indexes[i]];
                positions[i] = new Vector3Int(data.positions[i].x, data.positions[i].y, 0);
            }
            
            tileProcessingQueue.Enqueue(new TileProcessingQueueData {length = arraySize, tiles = tileBases, positions = positions});
        }
    }

    async void ProcessTileQueue()
    {
        while (true)
        {
            while(tileProcessingQueue.Count == 0)
                await Task.Yield();

            TileProcessingQueueData data = tileProcessingQueue.Dequeue();

            int segment = data.length / 64;
            for (int i = 0; i < 64; i++)
            {
                ArraySegment<Vector3Int> positionSegment = new ArraySegment<Vector3Int>(data.positions, i * segment, segment);
                ArraySegment<TileBase> tilesSegment = new ArraySegment<TileBase>(data.tiles, i * segment, segment);
                tilemap.SetTiles(positionSegment.ToArray(), tilesSegment.ToArray());
                await Task.Yield();
            }
        }
    }
}