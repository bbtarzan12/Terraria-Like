using System;
using UnityEngine;

namespace XNode.Noise
{
 
    [Serializable]
    public class PerlinNode : NoiseNode
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input;
        [Output] public NoiseNode output;
        public float x;
        public float y;
        public float scale = 10;
        [Range(0, 1)] public float threshold = 0.5f;
        [Range(1, 10)] public int fractal = 1;
        public Color color;
        public MixType mixType = MixType.SUB;

        protected override void Init()
        {
            base.Init(); 
            shader = Shader.Find("Noise/Perlin");
        }

        public override Texture2D GetTexture()
        {
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture", ((NoiseNode)GetPort(nameof(input)).Connection.node).GetTexture());
            noiseMaterial.SetFloat("_X", x);
            noiseMaterial.SetFloat("_Y", y);
            noiseMaterial.SetFloat("_Scale", scale);
            noiseMaterial.SetFloat("_Threshold", threshold);
            noiseMaterial.SetColor("_Color", color);
            noiseMaterial.SetInt("_Fractal", fractal);
            noiseMaterial.SetFloat("_Ratio", GetGraph.Ratio);
            noiseMaterial.SetInt("_Mode", (int)mixType);
            return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
        }

        public override void GenerateTexture()
        {
            if (!IsShaderInit)
                return;

            if (!Dirty)
                return;

            NoiseNode inputNoise = GetInputValue<NoiseNode>("input");
            
            if (inputNoise == null || inputNoise.Texture == null)
                return;
           
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture", inputNoise.Texture);
            noiseMaterial.SetFloat("_X", x);
            noiseMaterial.SetFloat("_Y", y);
            noiseMaterial.SetFloat("_Scale", scale);
            noiseMaterial.SetFloat("_Threshold", threshold);
            noiseMaterial.SetColor("_Color", color);
            noiseMaterial.SetInt("_Fractal", fractal);
            noiseMaterial.SetFloat("_Ratio", GetGraph.Ratio);
            noiseMaterial.SetInt("_Mode", (int)mixType);

            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;
        }

        public override void Update() => Dirty = true;
    }

}