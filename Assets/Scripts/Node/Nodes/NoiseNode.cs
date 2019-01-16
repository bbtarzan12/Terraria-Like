using System;
using UnityEngine;

namespace XNode.Noise
{

    public enum MixType { ADD, SUB, MUL, DIV }
    
    public abstract class NoiseNode : Node
    {
        protected Shader shader;

        public bool IsShaderInit => shader != null;
        public Texture2D Texture { get; protected set; }
        public bool Dirty { get; protected set; }

        public abstract Texture2D GetTexture();
        public abstract void GenerateTexture();
        public abstract void Update();
        
        public override object GetValue(NodePort port) => this;

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
            Texture = null;
            Update();
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            Update();
        }

        public void SetTextureDirty() => Dirty = true;
        public NoiseGraph GetGraph => (NoiseGraph) graph;

    }

}