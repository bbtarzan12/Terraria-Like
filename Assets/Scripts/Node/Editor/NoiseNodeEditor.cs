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

        Rect lastRect = GUILayoutUtility.GetLastRect();
        Rect toggleRect = new Rect(lastRect)
        {
            width = 18,
            height = 18,
            y = lastRect.y + EditorGUIUtility.singleLineHeight,
            x = (GetWidth() - 18f) / 2
        };

        noiseNode.ShowTextureInEditor = EditorGUI.Toggle(toggleRect, noiseNode.ShowTextureInEditor, NodeEditorResources.styles.preview);
        GUILayoutUtility.GetRect(toggleRect.width, toggleRect.height);
        
        if (noiseNode.ShowTextureInEditor)
        {
            if (noiseNode.HasTexture)
            {
                Rect textureRect = new Rect(toggleRect)
                {
                    width = GetWidth() * 0.95f,
                    height = GetWidth() * 0.8f / noiseNode.GetGraph.Ratio,
                    y = toggleRect.y + EditorGUIUtility.singleLineHeight * 2
                };
                textureRect.x = (GetWidth() - textureRect.width) / 2;
                GUILayoutUtility.GetRect(textureRect.width, textureRect.height + EditorGUIUtility.singleLineHeight);
                EditorGUI.DrawPreviewTexture(textureRect, noiseNode.Texture);
            }   
        }
        
        if (EditorGUI.EndChangeCheck())
        {
            noiseNode.Update();
        }
    }
    
    public override int GetWidth()
    {
        NoiseNode noiseNode = (NoiseNode) target;
        if(noiseNode != null && noiseNode.HasTexture && noiseNode.ShowTextureInEditor)
            return (int)(150 * noiseNode.GetGraph.Ratio);
        return (int)(base.GetWidth() * 1.2f);
    }
}