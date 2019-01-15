using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PerlinNoise : INoise
{
    public Shader Shader { get; private set; }
    public List<string> Property { get; } = new List<string>();
    public float Threshold { get; } = 0;

    public void Init()
    {
        Shader = Shader.Find("Noise/Perlin");
        Property.Add("Seed X");
        Property.Add("Seed Y");
        Property.Add("Fractal");
    }
    

}