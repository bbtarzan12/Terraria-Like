using System;
using UnityEngine;

namespace XNode.Noise.Math
{
 
    [Serializable]
    public class InvertNode : NoiseNode
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input;
        [Output] public NoiseNode output;

        protected override void Init()
        {
            shader = Shader.Find("Noise/Invert");
            base.Init(); 
        }

        public override Texture2D GetTexture()
        {
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture", ((NoiseNode)GetPort(nameof(input)).Connection.node).GetTexture());
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

            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;
        }
    }

}