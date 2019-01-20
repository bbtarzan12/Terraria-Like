using System;
using UnityEngine;
using XNode;
using XNode.Noise;
using XNode.Noise.Master;

[Serializable, CreateAssetMenu]
public class NoiseGraph : NodeGraph
{
    public Vector2Int mapSize = Vector2Int.one;
    public float Ratio => (float)mapSize.x / mapSize.y;

    public Texture2D GetTexture()
    {
        OutNode outNode = nodes.Find(n => n is OutNode) as OutNode;
            
        if (outNode == null)
        {
            Debug.Log($"{nameof(outNode)} is null");
            return null;
        }

        return outNode.GetTexture();
    }

    public void ApplyMapSize()
    {
        OutNode outNode = nodes.Find(n => n is OutNode) as OutNode;

        if (outNode == null)
        {
            Debug.Log($"{nameof(outNode)} is null");
            return;
        }

        foreach (var node in nodes)
        {
            var noiseNode = node as NoiseNode;

            if (noiseNode != null)
            {
                noiseNode.RefreshTexture();
                noiseNode.SetTextureDirty();
            }
        }

        outNode.GenerateTexture();
    }

    public void ExpandAllNodes()
    {
        foreach (var node in nodes)
        {
            var noiseNode = node as NoiseNode;

            if (noiseNode != null)
            {
                noiseNode.ShowTextureInEditor = true;
            }
        }
    }

    public void CollapseAllNodes()
    {
        foreach (var node in nodes)
        {
            var noiseNode = node as NoiseNode;

            if (noiseNode != null)
            {
                noiseNode.ShowTextureInEditor = false;
            }
        }
    }

}
