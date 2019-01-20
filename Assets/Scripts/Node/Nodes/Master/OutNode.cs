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