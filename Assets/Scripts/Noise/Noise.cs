using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[Serializable]
public class Noise
{
    public enum NoiseType { Perlin, Simplex, Circle, Box, Ridge, BasicTerrain }
    public enum NoiseValueType { Int, Float, String, Mode}
    public enum NoiseModeType { Sub, Add }

    public Shader Shader { get; private set; }
    public float Threshold => threshold;
    public NoiseType Type => type;

    public StringNoiseValueTypeDictionary Properties => properties;
    public StringStringDictionary Values => values;
    public Texture2D Texture => texture == null ? Texture2D.blackTexture : texture;
    public bool TextureDirty { get; private set; } = false;

    [SerializeField] NoiseType type;
    [SerializeField] float threshold;
    [SerializeField] Texture2D texture;
    [SerializeField] StringStringDictionary values;
    [SerializeField] StringNoiseValueTypeDictionary properties;

    Vector2Int mapsize;
    Texture2D beforeTexture;

    public Noise(NoiseType noiseType)
    {
        type = noiseType;
    }

    public void Update(Vector2Int mapsize, Texture2D beforeTexture)
    {
        properties = new StringNoiseValueTypeDictionary();
        this.beforeTexture = beforeTexture;
        this.mapsize = mapsize;
        switch (type)
        {
            case NoiseType.Perlin:
                Shader = Shader.Find("Noise/Perlin");
                Properties.Add("X", NoiseValueType.Float);
                Properties.Add("Y", NoiseValueType.Float);
                Properties.Add("Scale", NoiseValueType.Float);
                Properties.Add("Fractal", NoiseValueType.Int);
                Properties.Add("Mode", NoiseValueType.Mode);
                break;
            case NoiseType.Simplex:
                Shader = Shader.Find("Noise/Simplex");
                Properties.Add("X", NoiseValueType.Float);
                Properties.Add("Y", NoiseValueType.Float);
                Properties.Add("Scale", NoiseValueType.Float);
                Properties.Add("Fractal", NoiseValueType.Int);
                Properties.Add("Mode", NoiseValueType.Mode);
                break;
            case NoiseType.Circle:
                Shader = Shader.Find("Noise/Circle");
                Properties.Add("X", NoiseValueType.Float);
                Properties.Add("Y", NoiseValueType.Float);
                Properties.Add("Scale", NoiseValueType.Float);
                Properties.Add("Mode", NoiseValueType.Mode);
                break;
            case NoiseType.Box:
                Shader = Shader.Find("Noise/Box");
                Properties.Add("X", NoiseValueType.Float);
                Properties.Add("Y", NoiseValueType.Float);
                Properties.Add("BLX", NoiseValueType.Float);
                Properties.Add("BLY", NoiseValueType.Float);
                Properties.Add("TRX", NoiseValueType.Float);
                Properties.Add("TRY", NoiseValueType.Float);
                Properties.Add("Mode", NoiseValueType.Mode);
                break;
            case NoiseType.BasicTerrain:
                Shader = Shader.Find("Noise/BasicTerrain");
                Properties.Add("Height", NoiseValueType.Float);
                Properties.Add("Scale", NoiseValueType.Float);
                Properties.Add("Fractal", NoiseValueType.Int);
                Properties.Add("Mode", NoiseValueType.Mode);
                break;
            case NoiseType.Ridge:
                Shader = Shader.Find("Noise/Ridge");
                Properties.Add("Scale", NoiseValueType.Float);
                Properties.Add("Grain", NoiseValueType.Float);
                Properties.Add("Lacunarity", NoiseValueType.Float);
                Properties.Add("Fractal", NoiseValueType.Int);
                Properties.Add("Mode", NoiseValueType.Mode);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        foreach (var pair in Properties)
        {
            if(values.ContainsKey(pair.Key))
                continue;
            
            switch (pair.Value)
            {
                case NoiseValueType.Int:
                    values.Add(pair.Key, "0");
                    break;
                case NoiseValueType.Float:
                    values.Add(pair.Key, "0");
                    break;
                case NoiseValueType.String:
                    values.Add(pair.Key, "");
                    break;
                case NoiseValueType.Mode:
                    values.Add(pair.Key, "0");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public void DrawTexture()
    {
        if (!TextureDirty)
            return;
        
        texture = new Texture2D(mapsize.x, mapsize.y, TextureFormat.RGBA32, false);
        
        Material noiseMaterial = new Material(Shader);
        noiseMaterial.SetTexture("_Texture", beforeTexture);
        noiseMaterial.SetFloat("_Threshold", threshold);
        foreach (var pair in properties)
        {
            switch (pair.Value)
            {
                case NoiseValueType.Mode:
                case NoiseValueType.Int:
                    noiseMaterial.SetInt("_"+pair.Key, int.Parse(values[pair.Key]));
                    break;
                case NoiseValueType.Float:
                    noiseMaterial.SetFloat("_"+pair.Key, float.Parse(values[pair.Key]));
                    break;
                case NoiseValueType.String:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        RenderTexture sampleTexture = RenderTexture.GetTemporary(mapsize.x, mapsize.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);   

        Graphics.Blit(Texture2D.whiteTexture, sampleTexture, noiseMaterial);
        RenderTexture.active = sampleTexture;
        texture.ReadPixels(new Rect(0, 0, mapsize.x, mapsize.y), 0, 0);
        texture.Apply();
        RenderTexture.ReleaseTemporary(sampleTexture);
        Object.DestroyImmediate(noiseMaterial);
        TextureDirty = false;
    }

    public void SetDirty()
    {
        TextureDirty = true;
    }
}