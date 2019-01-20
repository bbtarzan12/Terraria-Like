
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

        public override void GenerateTexture()
        {
            if (!Dirty)
                return;
            
            Texture = GetTexture();
        }
    }
}