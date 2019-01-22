using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using XNode.Noise.Simple;
using XNode.Noise.Util;
using XNodeEditor;

[CustomNodeEditor(typeof(ColorNode))]
public class ColorNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {

        Dictionary<string, Color> oreColors = new Dictionary<string, Color>();
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in a.GetTypes())
            {
                if(t == typeof(RuleTile))
                {
                    
                }
            }
        }
        EditorGUI.BeginChangeCheck();
        
        ColorNode noiseNode = (ColorNode) target;
        
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
        ColorNode noiseNode = (ColorNode) target;
        if(noiseNode != null && noiseNode.HasTexture && noiseNode.ShowTextureInEditor)
            return (int)(150 * noiseNode.GetGraph.Ratio);
        return (int)(base.GetWidth() * 1.2f);
    }
    
}