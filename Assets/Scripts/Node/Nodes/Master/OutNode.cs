using System.IO;
using UnityEngine;

namespace XNode.Noise.Master
{

    public class OutNode : NoiseNode
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input;

        public Texture2D GetInputTexture => GetInputValue<NoiseNode>("input")?.Texture;
        public bool HasTexture => GetInputValue<NoiseNode>("input")?.Texture != null;
        public override Texture2D GetTexture() =>((NoiseNode)GetPort(nameof(input))?.Connection?.node)?.GetTexture();

        public override void GenerateTexture()
        {            
            if (!Dirty)
                return;

            Texture = GetInputValue<NoiseNode>("input")?.Texture;           
            Dirty = false;
        }
    }
}