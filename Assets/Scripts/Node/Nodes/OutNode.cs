using System.IO;
using UnityEngine;

namespace XNode.Noise
{

    public class OutNode : NoiseNode
    {
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input;

        public override Texture2D GetTexture()
        {
            return ((NoiseNode)GetPort(nameof(input)).Connection.node).GetTexture();
        }

        public override void GenerateTexture()
        {            
            if (!Dirty)
                return;
            
            NoiseNode inputNoise = GetInputValue<NoiseNode>("input");
            if (inputNoise == null || inputNoise.Texture == null)
                return;
            
            Texture = inputNoise.Texture;           
            Dirty = false;
        }

        public override void Update() => Dirty = true;
    }
}