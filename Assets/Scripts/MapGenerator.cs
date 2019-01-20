using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] TileBase redTile;
    [SerializeField] TileBase greenTile;
    [SerializeField] TileBase blueTile;
    [SerializeField] TileBase yellowTile;
    [SerializeField] TileBase whiteTile;
    [SerializeField] NoiseGraph graph;
    Tilemap tilemap;

    public static async void InitMap(Vector2Int mapSize, Texture2D texture, Tilemap tilemap, TileBase redTile, TileBase greenTile, TileBase blueTile, TileBase yellowTile, TileBase whiteTile)
    {
        float time = Time.time;
        Debug.Log($"Start {nameof(InitMap)}");
        for (int x = 0; x < mapSize.x; x++)
        {
            Debug.Log($"Init Map : {((float)x / mapSize.x):P}");
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);

                Color color = texture.GetPixel(x, y);
                const float threshold = 0.3f;
                if(color.r > threshold && color.g > threshold && color.b > threshold)
                    tilemap.SetTile(tilePos, whiteTile);
                else if(color.r > threshold && color.g > threshold)
                    tilemap.SetTile(tilePos, yellowTile);
                else if(color.r > threshold)
                    tilemap.SetTile(tilePos, redTile);
                else if(color.g > threshold)
                    tilemap.SetTile(tilePos, greenTile);
                else if(color.b > threshold)
                    tilemap.SetTile(tilePos, blueTile);
                else
                    tilemap.SetTile(tilePos, null);
            }

            await Task.Yield();
        }
        Debug.Log($"End {nameof(InitMap)} {(Time.time - time):F}");
    }

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void Start()
    {
        //int[,] map = GenerateMapArray(mapSize.x, mapSize.y, threshold, scale, seed);
        InitMap(graph.mapSize, graph.GetTexture(), tilemap, redTile, greenTile, blueTile, yellowTile, whiteTile);
    }

    void Update()
    {
//        map = GenerateMapTexture(mapSize.x, mapSize.y, perlinOptiln, simplexOption);
    }
}
