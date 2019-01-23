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
            shader = Shader.Find("Noise/Flip");
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture", ((NoiseNode)GetPort(nameof(input)).Connection.node).GetTexture());
            noiseMaterial.SetInt("_X", Convert.ToInt32(FlipX));
            noiseMaterial.SetInt("_Y", Convert.ToInt32(FlipY));
            return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
        }

        public override Texture2D GenerateTexture()
        {
            if (!IsShaderInit)
                return Texture2D.whiteTexture;

            if (!Dirty)
                return HasTexture ? Texture : GenerateTexture();

            NoiseNode inputNoise = GetInputValue<NoiseNode>("input");
            
            if (inputNoise == null)
                return Texture2D.whiteTexture;

            Texture2D inputTexture = inputNoise.Texture == null ? inputNoise.GenerateTexture() : inputNoise.Texture;
           
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture", inputTexture);
            noiseMaterial.SetInt("_X", Convert.ToInt32(FlipX));
            noiseMaterial.SetInt("_Y", Convert.ToInt32(FlipY));

            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;

            return Texture;
        }
    }

}