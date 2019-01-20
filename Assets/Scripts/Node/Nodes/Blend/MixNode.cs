using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise.Blend
{

    public class MixNode : NoiseNode
    {
        public enum MixType { ADD, SUB, MUL, DIV }
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

        public override void GenerateTexture()
        {
            if (!IsShaderInit)
                return;

            if (!Dirty)
                return;

            NoiseNode input1Noise = GetInputValue<NoiseNode>("input1");
            NoiseNode input2Noise = GetInputValue<NoiseNode>("input2");
            Vector2Int mapsize = GetGraph.mapSize;
            
            if (input1Noise == null || input1Noise.Texture == null)
                return;

            if (input2Noise == null || input2Noise.Texture == null)
                return;
            
            Texture = new Texture2D(GetGraph.mapSize.x, GetGraph.mapSize.y, TextureFormat.RGBA32, false);
            
            Material noiseMaterial = new Material(shader);
            noiseMaterial.SetTexture("_Texture1", input1Noise.Texture);
            noiseMaterial.SetTexture("_Texture2", input2Noise.Texture);
            noiseMaterial.SetInt("_Mode", (int)mixType);
            
            Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);  
            GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
            Dirty = false;
        }

        public override void Update() => Dirty = true;


    }
}