using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct PerlinOption
{
    public float scale;
    public float seed;
    public int fractal;
}

[Serializable]
public struct SimplexOption
{
    public float scale;
    public float seed;
    public int fractal;
}

public class MapGenerator : MonoBehaviour
{

    [SerializeField] Vector2Int mapSize;
    [SerializeField] [Range(0, 1)] float threshold;
    [SerializeField] TileBase tile;

    [SerializeField] PerlinOption perlinOptiln;
    [SerializeField] SimplexOption simplexOption;
    
    Tilemap tilemap;
    Texture2D map;
    
//    public static int[,] GenerateMapArray(int width, int height, float threshold, float scale, int seed)
//    {
//      
//        int[,] map = new int[width, height];
//        
//        for (int x = 0; x < map.GetUpperBound(0); x++)
//        {
//            for (int y = 0; y < map.GetUpperBound(1); y++)
//            {
//                float mapValue = SimplexNoise.Noise.CalcPixel2D(x, y, scale);
//                if(mapValue > threshold)
//                    map[x, y] = 1;
//            }
//        }
//
//        return map;
//    }
//
//    public static void InitMap(int[,] map, Tilemap tilemap, TileBase tile)
//    {
//        tilemap.ClearAllTiles();
//        
//        for (int x = 0; x < map.GetUpperBound(0); x++)
//        {
//            for (int y = 0; y < map.GetUpperBound(1); y++)
//            {
//                Vector3Int tilePos = new Vector3Int(x, y, 0);
//                switch (map[x, y])
//                {
//                    case 0:
//                        tilemap.SetTile(tilePos, null);
//                        break;
//                    case 1:
//                        tilemap.SetTile(tilePos, tile);
//                        break;
//                    default:
//                        tilemap.SetTile(tilePos, null);
//                        break;
//                }
//            }
//        }
//    }

    public static async void InitMap(Vector2Int mapSize, Texture2D texture, float threshold, Tilemap tilemap, TileBase tile)
    {
        float time = Time.time;
        Debug.Log($"Start {nameof(InitMap)}");
        for (int x = 0; x < mapSize.x; x++)
        {
            Debug.Log($"Init Map : {((float)x / mapSize.x):P}");
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if(texture.GetPixel(x, y).r > threshold)
                    tilemap.SetTile(tilePos, tile);
                else
                    tilemap.SetTile(tilePos, null);
            }

            await Task.Yield();
        }
        Debug.Log($"End {nameof(InitMap)} {(Time.time - time):F}");
    }

    public static Texture2D GenerateMapTexture(int width, int height, PerlinOption perlinOption, SimplexOption simplexOption)
    {
        float time = Time.time;
        Debug.Log($"Start {nameof(GenerateMapTexture)}");
        
        if (width > height)
            height = width;
        else
            width = height;
        
        Shader terrainShader = Shader.Find("Noise/Terrain");

        Texture2D resultTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture simpleTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        
        Material terrainMaterial = new Material(terrainShader);
        terrainMaterial.SetFloat("_SX", simplexOption.seed * width);
        terrainMaterial.SetFloat("_SY", simplexOption.seed * height);
        terrainMaterial.SetFloat("_SScale", simplexOption.scale);
        terrainMaterial.SetInt("_SFractal", simplexOption.fractal);
        terrainMaterial.SetFloat("_PX", perlinOption.seed * width);
        terrainMaterial.SetFloat("_PY", perlinOption.seed * height);
        terrainMaterial.SetFloat("_PScale", perlinOption.scale);
        terrainMaterial.SetInt("_PFractal", perlinOption.fractal);
        
        Graphics.Blit(null, simpleTexture, terrainMaterial);

        RenderTexture previousRenderTexture = RenderTexture.active;
        RenderTexture.active = simpleTexture;
        resultTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resultTexture.Apply();
        RenderTexture.active = previousRenderTexture;
        
        RenderTexture.ReleaseTemporary(simpleTexture); 
        Destroy(terrainMaterial);
        GC.Collect();
        
        Debug.Log($"End {nameof(GenerateMapTexture)} {(Time.time - time):F}");
        return resultTexture;
    }

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void Start()
    {
        //int[,] map = GenerateMapArray(mapSize.x, mapSize.y, threshold, scale, seed);
        map = GenerateMapTexture(mapSize.x, mapSize.y, perlinOptiln, simplexOption);
        InitMap(mapSize, map, threshold, tilemap, tile);
    }

    void Update()
    {
//        map = GenerateMapTexture(mapSize.x, mapSize.y, perlinOptiln, simplexOption);
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(100, 100, 500, 500), map);
    }
}
