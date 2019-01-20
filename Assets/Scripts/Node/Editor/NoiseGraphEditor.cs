using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode.Noise;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(NoiseGraph))]
public class NoiseGraphEditor : NodeGraphEditor
{
    public override string GetNodeMenuName(Type type)
    {
        return type.Namespace.Contains("XNode.Noise") ? base.GetNodeMenuName(type).Replace("X Node/Noise/", "") : null;
    }

    public override void OnGUI()
    {
        serializedObject.Update();
        
        
        GUIStyle backgroundStyle, headerStyle;

        backgroundStyle = new GUIStyle();
        backgroundStyle.normal.background = NodeEditorResources.nodeBody;
        backgroundStyle.border = new RectOffset(32, 32, 32, 32);
        backgroundStyle.padding = new RectOffset(16, 16, 4, 16);
        
        headerStyle = new GUIStyle();
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        
        
        GUILayout.BeginArea(new Rect(0, 0, 200, 200), backgroundStyle);
        GUILayout.Label("Graph Settings", headerStyle, GUILayout.Height(30));
        

        
        string[] excludes = { "m_Script", "nodes" };
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        EditorGUIUtility.labelWidth = 84;
        while (iterator.NextVisible(enterChildren)) {
            enterChildren = false;
            if (excludes.Contains(iterator.name)) continue;
            EditorGUILayout.PropertyField(iterator, true);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Apply MapSize"))
        {
            ((NoiseGraph)target).ApplyMapSize();
        }
        
        if (GUILayout.Button("Expand All Nodes"))
        {
            ((NoiseGraph)target).ExpandAllNodes();
        }
        
        if (GUILayout.Button("Collapse All Nodes"))
        {
            ((NoiseGraph)target).CollapseAllNodes();
        }
        
                
        GUILayout.EndArea();
        
        serializedObject.ApplyModifiedProperties();
    }
}