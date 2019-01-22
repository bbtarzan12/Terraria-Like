using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise.Blend
{

    public class MixNode : NoiseNode
    {
        public enum MixType { ADD, SUB, MUL, DIV, OVERADD, OVERMUL }
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input1;
        [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public NoiseNode input2;
        [Output] public NoiseNode output;
        public MixType mixType;

        protected override void Init()
        {
            base.Init();
            
            shader = Shader.Find("Noise/Mix");
        }

        public override Texture2D GetTexture()
        {
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture1", ((NoiseNode)GetPort(nameof(input1)).Connection.node).GetTexture());
            noiseMaterial.SetTexture("_Texture2", ((NoiseNode)GetPort(nameof(input2)).Connection.node).GetTexture());
            noiseMaterial.SetInt("_Mode", (int)mixType);
            
            return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);  
        }

        public override Texture2D GenerateTexture()
        {
            if (!IsShaderInit)
                return Texture2D.whiteTexture;

            if (!Dirty)
                return HasTexture ? Texture : GenerateTexture();

            NoiseNode input1Noise = GetInputValue<NoiseNode>("input1");
            NoiseNode input2Noise = GetInputValue<NoiseNode>("input2");
            
            if (input1Noise == null)
                return Texture2D.whiteTexture;
            
            if (input2Noise == null)
                return Texture2D.whiteTexture;
            
            Texture2D input1Texture = input1Noise.Texture == null ? input1Noise.GenerateTexture() : input1Noise.Texture;
            Texture2D input2Texture = input2Noise.Texture == null ? input2Noise.GenerateTexture() : input2Noise.Texture;
            
            Texture = new Texture2D(GetGraph.mapSize.x, GetGraph.mapSize.y, TextureFormat.RGBA32, false);
            
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture1", input1Texture);
            noiseMaterial.SetTexture("_Texture2", input2Texture);
            noiseMaterial.SetInt("_Mode", (int)mixType);
            
            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);  
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;

            return Texture;
        }

        public override void Update() => Dirty = true;


    }
}