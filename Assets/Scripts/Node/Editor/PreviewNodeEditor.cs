using UnityEditor;
using UnityEngine;
using XNode.Noise;
using XNodeEditor;

[CustomNodeEditor(typeof(PreviewNode))]
public class PreviewNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        NodeEditorGUILayout.PortField(target.GetPort("input"));
        
        Rect lastRect = GUILayoutUtility.GetLastRect();
        Rect textureRect = new Rect(lastRect)
        {
            width = 128,
            height = 128,
            y = lastRect.y + 2
        };
        
        PreviewNode noiseNode = target as PreviewNode;
        if(noiseNode != null && noiseNode.HasTexture)
            EditorGUI.DrawPreviewTexture(textureRect, noiseNode.GetTexture());
        
    }

    public override GUIStyle GetBodyStyle()
    {
        PreviewNode noiseNode = target as PreviewNode;
        if (noiseNode != null && noiseNode.HasTexture)
        {
            GUIStyle nodeBody = new GUIStyle();
            nodeBody.normal.background = NodeEditorResources.nodeBody;
            nodeBody.border = new RectOffset(32, 32, 32, 32);
            nodeBody.padding = new RectOffset(16, 16, 4, 16);
            nodeBody.fixedHeight = 180;    
            return nodeBody;   
        }
        return base.GetBodyStyle();
    }

    public override int GetWidth()
    {
        PreviewNode noiseNode = target as PreviewNode;
        if(noiseNode != null && noiseNode.HasTexture)
            return 160;
        return base.GetWidth();
    }
}