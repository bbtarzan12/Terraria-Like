using UnityEditor;
using UnityEngine;
using XNode.Noise;
using XNodeEditor;

[CustomNodeEditor(typeof(NoiseNode))]
public class NoiseNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        NoiseNode noiseNode = (NoiseNode) target;
        if(noiseNode.Dirty)
            noiseNode.GenerateTexture();
        
        base.OnBodyGUI();
        
        if (EditorGUI.EndChangeCheck())
        {
            noiseNode.Update();
        }
    }
}