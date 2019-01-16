using UnityEditor;
using UnityEngine;
using XNode.Noise;
using XNodeEditor;

[CustomNodeEditor(typeof(ColorNode))]
public class ColorNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        NoiseNode noiseNode = (NoiseNode) target;
        if(noiseNode.Dirty)
            noiseNode.GenerateTexture();
        
        NodeEditorGUILayout.PortField(target.GetPort("output"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("fillColor"));
    }
}