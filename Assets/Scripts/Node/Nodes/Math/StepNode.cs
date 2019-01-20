using System;
using UnityEngine;

namespace XNode.Noise.Math
{
 
    [Serializable]
    public class StepNode : NoiseNode
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input;
        [Output] public NoiseNode output;
        [Range(0, 1)] public float threshold;

        protected override void Init()
        {
            base.Init(); 
            shader = Shader.Find("Noise/Step");
        }

        public override Texture2D GetTexture()
        {
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture", ((NoiseNode)GetPort(nameof(input)).Connection.node).GetTexture());
            noiseMaterial.SetFloat("_Threshold", threshold);
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
            noiseMaterial.SetFloat("_Threshold", threshold);
            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;
        }
    }

}