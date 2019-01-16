
using System;
using UnityEngine;

namespace XNode.Noise
{
    [Serializable]
    public class PreviewNode : NoiseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseNode input;

        public bool HasTexture => GetInputValue<NoiseNode>("input")?.Texture != null;
        public override Texture2D GetTexture() => GetInputValue<NoiseNode>("input")?.Texture;

        public override void GenerateTexture()
        {
            if (!Dirty)
                return;
            
            Texture = GetTexture();
        }

        public override void Update() => Dirty = true;
    }
}