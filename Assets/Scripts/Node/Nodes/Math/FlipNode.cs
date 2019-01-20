using System;
using UnityEngine;

namespace XNode.Noise.Math
{
 
    [Serializable]
    public class FlipNode : NoiseNode
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input;
        [Output] public NoiseNode output;
        public bool FlipX = false;
        public bool FlipY = false;

        protected override void Init()
        {
            shader = Shader.Find("Noise/Flip");
            base.Init(); 
        }

        public override Texture2D GetTexture()
        {
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture", ((NoiseNode)GetPort(nameof(input)).Connection.node).GetTexture());
            noiseMaterial.SetInt("_X", Convert.ToInt32(FlipX));
            noiseMaterial.SetInt("_Y", Convert.ToInt32(FlipY));
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
            noiseMaterial.SetInt("_X", Convert.ToInt32(FlipX));
            noiseMaterial.SetInt("_Y", Convert.ToInt32(FlipY));

            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;
        }
    }

}