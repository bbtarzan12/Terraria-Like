using System;
using UnityEngine;
using XNode;
using XNode.Noise;

[Serializable, CreateAssetMenu]
public class NoiseGraph : NodeGraph
{
    public Vector2Int mapSize;

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

    public void GenerateTexture()
    {
        foreach (var node in nodes)
        {
            var noiseNode = node as NoiseNode;
                
            if (noiseNode != null)
            {
                noiseNode.SetTextureDirty();
            }
        }
    }
}
