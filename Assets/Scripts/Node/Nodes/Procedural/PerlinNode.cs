using System;
using UnityEngine;

namespace XNode.Noise.Procedural
{
 
    [Serializable]
    public class PerlinNode : NoiseNode
    {
        [Output] public NoiseNode output;
        public Vector2 offset;
        public float scale = 10;
        [Range(1, 10)] public int fractal = 1;

        protected override void Init()
        {
            shader = Shader.Find("Noise/Perlin");
            base.Init(); 
        }

        public override Texture2D GetTexture()
        {
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetFloat("_X", offset.x);
            noiseMaterial.SetFloat("_Y", offset.y);
            noiseMaterial.SetFloat("_Scale", scale);
            noiseMaterial.SetInt("_Fractal", fractal);
            noiseMaterial.SetFloat("_Ratio", GetGraph.Ratio);
            return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
        }

        public override Texture2D GenerateTexture()
        {
            if (!IsShaderInit)
                return Texture2D.whiteTexture;

            if (!Dirty)
                return HasTexture ? Texture : GenerateTexture();
            
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetFloat("_X", offset.x);
            noiseMaterial.SetFloat("_Y", offset.y);
            noiseMaterial.SetFloat("_Scale", scale);
            noiseMaterial.SetInt("_Fractal", fractal);
            noiseMaterial.SetFloat("_Ratio", GetGraph.Ratio);
            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;
            
            return Texture;
        }
    }

}