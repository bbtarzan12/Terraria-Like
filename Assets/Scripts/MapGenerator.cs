using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] TileBase tile;

    NoiseGenerator noiseGenerator;
    Tilemap tilemap;

    public static async void InitMap(Vector2Int mapSize, Texture2D texture, Tilemap tilemap, TileBase tile)
    {
        float time = Time.time;
        Debug.Log($"Start {nameof(InitMap)}");
        for (int x = 0; x < mapSize.x; x++)
        {
            Debug.Log($"Init Map : {((float)x / mapSize.x):P}");
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if(texture.GetPixel(x, y).r > 0.98f)
                    tilemap.SetTile(tilePos, tile);
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
        noiseGenerator = GetComponent<NoiseGenerator>();
    }

    void Start()
    {
        //int[,] map = GenerateMapArray(mapSize.x, mapSize.y, threshold, scale, seed);
        InitMap(noiseGenerator.MapSize, noiseGenerator.MapTexture, tilemap, tile);
    }

    void Update()
    {
//        map = GenerateMapTexture(mapSize.x, mapSize.y, perlinOptiln, simplexOption);
    }
}
