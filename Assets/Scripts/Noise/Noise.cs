using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[Serializable]
public class Noise
{
    public enum NoiseType { Perlin, Simplex }
    public enum NoiseValueType { Int, Float, String }

    public Shader Shader { get; private set; }
    public float Threshold => threshold;
    public NoiseType Type => type;

    public StringNoiseValueTypeDictionary Properties => properties;
    public StringStringDictionary Values => values;
    public Texture2D Texture => texture;
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
                Properties.Add("X Seed", NoiseValueType.Float);
                Properties.Add("Y Seed", NoiseValueType.Float);
                Properties.Add("Scale", NoiseValueType.Float);
                Properties.Add("Fractal", NoiseValueType.Int);
                break;
            case NoiseType.Simplex:
                Shader = Shader.Find("Noise/Simplex");
                Properties.Add("X Seed", NoiseValueType.Float);
                Properties.Add("Y Seed", NoiseValueType.Float);
                Properties.Add("Scale", NoiseValueType.Float);
                Properties.Add("Fractal", NoiseValueType.Int);
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
        
        //CommandBuffer buffer = new CommandBuffer();
        Material noiseMaterial = new Material(Shader);
        noiseMaterial.SetTexture("_Texture", beforeTexture);
        noiseMaterial.SetFloat("_Threshold", threshold);
        foreach (var pair in properties)
        {
            switch (pair.Value)
            {
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
//        RenderTexture previousRenderTexture = RenderTexture.active;
        RenderTexture.active = sampleTexture;
        texture.ReadPixels(new Rect(0, 0, mapsize.x, mapsize.y), 0, 0);
        texture.Apply();
//        RenderTexture.active = previousRenderTexture;
        
        RenderTexture.ReleaseTemporary(sampleTexture);
        Object.DestroyImmediate(noiseMaterial);
        TextureDirty = false;
    }

    public void SetDirty()
    {
        TextureDirty = true;
    }
}