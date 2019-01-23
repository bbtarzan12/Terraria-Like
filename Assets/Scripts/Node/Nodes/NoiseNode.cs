using System;
using UnityEngine;

namespace XNode.Noise
{    
    public abstract class NoiseNode : Node
    {
        protected Shader shader;

        public bool IsShaderInit => shader != null;
        public Texture2D Texture { get; protected set; }
        public bool Dirty { get; protected set; }
        public bool HasTexture => Texture != null;
        public bool ShowTextureInEditor { get; set; }

        public abstract Texture2D GetTexture();
        public abstract Texture2D GenerateTexture();
        public virtual void Update() => Dirty = true;
        
        public override object GetValue(NodePort port) => this;

        protected override void Init()
        {
            Texture = null;
            Update();
            base.Init();
        }

        public void ForceInit() => Init();
        
        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
            Texture = null;
            Update();
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            Texture = null;
            Update();
        }

        public void SetTextureDirty() => Dirty = true;
        public NoiseGraph GetGraph => (NoiseGraph) graph;
        public void RefreshTexture() => DestroyImmediate(Texture);

    }

}