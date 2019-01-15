using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseEditor : Editor
{

    NoiseGenerator noise;
    SerializedProperty mapsize;
    ReorderableList noiseList;

    void OnEnable()
    {
        noise = (NoiseGenerator) target;
        mapsize = serializedObject.FindProperty("mapsize");
        if(noise.Noises == null)
            noise.Noises = new List<INoise>();
        noiseList = new ReorderableList(noise.Noises, typeof(INoise));
        noiseList.drawElementCallback = DrawElementCallback;
        noiseList.onAddDropdownCallback = OnAddDropdownCallback;
    }

    void OnAddDropdownCallback(Rect buttonrect, ReorderableList list)
    {
        GenericMenu menu = new GenericMenu();

        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(INoise).IsAssignableFrom(p) && !p.IsInterface);

        foreach (var type in types)
        {
            menu.AddItem(new GUIContent(type.ToString()), false, () => { noise.Noises.Add((INoise) Activator.CreateInstance(type)); });
        }
        menu.DropDown(buttonrect);
    }

    void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        var element = noise.Noises[index];
        rect.height -= 4;
        rect.y += 2;
        EditorGUI.LabelField(rect, element.GetType().ToString());
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mapsize, new GUIContent("Map Size"));
        noiseList.DoLayoutList();
        
        serializedObject.ApplyModifiedProperties();
    }
}