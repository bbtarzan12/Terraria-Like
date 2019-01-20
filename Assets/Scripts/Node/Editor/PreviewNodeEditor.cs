using UnityEditor;
using UnityEngine;
using XNode.Noise.Util;
using XNodeEditor;

[CustomNodeEditor(typeof(PreviewNode))]
public class PreviewNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        NodeEditorGUILayout.PortField(target.GetPort("input"));
        
        PreviewNode noiseNode = target as PreviewNode;
        if (noiseNode != null && noiseNode.HasTexture)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect textureRect = new Rect(lastRect)
            {
                width = GetWidth() * 0.95f,
                height = GetWidth() * 0.8f / noiseNode.GetGraph.Ratio,
                y = lastRect.y + EditorGUIUtility.singleLineHeight * 2
            };
            textureRect.x = (GetWidth() - textureRect.width) / 2;
            GUILayoutUtility.GetRect(textureRect.width, textureRect.height + EditorGUIUtility.singleLineHeight);
            EditorGUI.DrawPreviewTexture(textureRect, noiseNode.GetInputTexture);
        }

    }
    
    public override int GetWidth()
    {
        PreviewNode noiseNode = target as PreviewNode;
        if(noiseNode != null && noiseNode.HasTexture)
            return (int)(150 * noiseNode.GetGraph.Ratio);
        return base.GetWidth();
    }
}