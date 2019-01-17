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
        
        PreviewNode noiseNode = target as PreviewNode;
        if (noiseNode != null && noiseNode.HasTexture)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect textureRect = new Rect(lastRect)
            {
                width = 128 * noiseNode.GetGraph.Ratio,
                height = 128,
                y = lastRect.y + 2
            };
            textureRect.x = (GetWidth() - textureRect.width) / 2;
            textureRect.y = (GetBodyStyle().fixedHeight - textureRect.height) / 2 + 20;
            EditorGUI.DrawPreviewTexture(textureRect, noiseNode.GetTexture());

        }

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
            nodeBody.fixedHeight = 200;    
            return nodeBody;   
        }
        return base.GetBodyStyle();
    }

    public override int GetWidth()
    {
        PreviewNode noiseNode = target as PreviewNode;
        if(noiseNode != null && noiseNode.HasTexture)
            return (int)(160 * noiseNode.GetGraph.Ratio);
        return base.GetWidth();
    }
}