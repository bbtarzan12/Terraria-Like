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

[CustomNodeEditor(typeof(OreColorNode))]
public class OreColorNodeEditor : NodeEditor
{
    static StringColorDictionary oreColors;
    
    public override void OnBodyGUI()
    {

        if (oreColors == null || oreColors.Count == 0)
        {
            oreColors = new StringColorDictionary();
            string[] assets = AssetDatabase.FindAssets("t:Ruletile");
            foreach (var asset in assets)
            {
                RuleTile ruleTile = AssetDatabase.LoadAssetAtPath<RuleTile>(AssetDatabase.GUIDToAssetPath(asset));
                oreColors.Add(ruleTile.name, ruleTile.color);
            }   
        }
        
        EditorGUI.BeginChangeCheck();
        
        base.OnBodyGUI();
        
        OreColorNode noiseNode = (OreColorNode) target;
        
        if(noiseNode.Dirty)
            noiseNode.GenerateTexture();

        noiseNode.oreIndex = EditorGUILayout.Popup("Ore", noiseNode.oreIndex, oreColors.Keys.ToArray());
        
        EditorGUI.BeginDisabledGroup(true);
        noiseNode.color = EditorGUILayout.ColorField("Ore Color", oreColors.Values.ToArray()[noiseNode.oreIndex]);
        EditorGUI.EndDisabledGroup();
        
        if (EditorGUI.EndChangeCheck())
        {
            noiseNode.Update();
        }
    }
}