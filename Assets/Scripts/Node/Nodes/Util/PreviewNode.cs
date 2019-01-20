
using System;
using UnityEngine;

namespace XNode.Noise.Util
{
    [Serializable]
    public class PreviewNode : NoiseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseNode input;
        
        public Texture2D GetInputTexture => GetInputValue<NoiseNode>("input")?.Texture;
        public bool HasTexture => GetInputValue<NoiseNode>("input")?.Texture != null;
        public override Texture2D GetTexture() => GetInputValue<NoiseNode>("input")?.Texture;

        public override Texture2D GenerateTexture()
        {            
            if (!Dirty)
                return Texture != null ? Texture : GenerateTexture();
            
            NoiseNode inputNoise = GetInputValue<NoiseNode>("input");

            if (inputNoise == null)
                return Texture2D.whiteTexture;

            Texture = inputNoise.Texture == null ? inputNoise.GenerateTexture() : inputNoise.Texture;        
            Dirty = false;
            
            return Texture;
        }
    }
}