using System;
using UnityEngine;

namespace XNode.Noise.Procedural
{
 
    [Serializable]
    public class RidgedMultiNode : NoiseNode
    {
        [Output] public NoiseNode output;
        public Vector2 offset;
        public float ridgedOffset = 0.5f;
        public float scale = 10;
        public float lacunarity = 1;
        public float gain = 3;
        [Range(1, 10)] public int fractal = 3;

        protected override void Init()
        {
            shader = Shader.Find("Noise/RidgedMulti");
            base.Init(); 
        }

        public override Texture2D GetTexture()
        {
            shader = Shader.Find("Noise/RidgedMulti");
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetFloat("_X", offset.x);
            noiseMaterial.SetFloat("_Y", offset.y);
            noiseMaterial.SetFloat("_RidgedOffset", ridgedOffset);
            noiseMaterial.SetFloat("_Lacunarity", lacunarity);
            noiseMaterial.SetFloat("_Gain", gain);
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
            noiseMaterial.SetFloat("_RidgedOffset", ridgedOffset);
            noiseMaterial.SetFloat("_Lacunarity", lacunarity);
            noiseMaterial.SetFloat("_Gain", gain);
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