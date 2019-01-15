using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseEditor : Editor
{

    NoiseGenerator generator;
    SerializedProperty mapsize;
    SerializedProperty noises;
    ReorderableList noiseList;

    void OnEnable()
    {
        generator = (NoiseGenerator)target;
        mapsize = serializedObject.FindProperty("mapsize");
        noises = serializedObject.FindProperty("noises");
        noiseList = new ReorderableList(serializedObject, noises);
        noiseList.elementHeight = 180;
        noiseList.drawElementCallback = DrawElementCallback;
        noiseList.onAddDropdownCallback = OnAddDropdownCallback;
    }

    void OnAddDropdownCallback(Rect buttonrect, ReorderableList list)
    {
        GenericMenu menu = new GenericMenu();

        var noiseTypes = Enum.GetNames(typeof(Noise.NoiseType));

        foreach (var noiseType in noiseTypes)
        {
            menu.AddItem(new GUIContent(noiseType), false, () => 
            {
                generator.Noises.Add(new Noise((Noise.NoiseType)Enum.Parse(typeof(Noise.NoiseType), noiseType)));
            });
        }
        menu.DropDown(buttonrect);
    }

    void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        EditorGUI.BeginChangeCheck();
        var noise = generator.Noises[index];
        var noiseProperty = noises.GetArrayElementAtIndex(index);
        var thresholdProperty = noiseProperty.FindPropertyRelative("threshold");
        
        EditorGUIUtility.labelWidth = 50;
        rect.height = EditorGUIUtility.singleLineHeight;        
        
        var textureRect = new Rect(rect)
        {
            width = 128,
            height = 128,
            y = rect.y + 2
        };
        
        var nameRect = new Rect(rect)
        {
            width = rect.width - 130,
            x = rect.x + 130,
            y = rect.y + 2
        };

        var shaderRect = new Rect(nameRect)
        {
            y = nameRect.y + EditorGUIUtility.singleLineHeight + 2
        };
        
        var thresholdRect = new Rect(shaderRect)
        {
            y = shaderRect.y + EditorGUIUtility.singleLineHeight + 2
        };

        var valueRect = new Rect(thresholdRect)
        {
            y = thresholdRect.y + EditorGUIUtility.singleLineHeight + 2
            
        };

        EditorGUI.DrawPreviewTexture(textureRect, noise.Texture);    
        EditorGUI.LabelField(nameRect, noise.Type.ToString());
        EditorGUI.Slider(thresholdRect, thresholdProperty, 0, 1);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.ObjectField(shaderRect, noise.Shader, typeof(Shader), false);
        EditorGUI.EndDisabledGroup();

        foreach (var pair in noise.Properties)
        {               
            switch (pair.Value)
            {
                case Noise.NoiseValueType.Int:
                    noise.Values[pair.Key] = EditorGUI.IntField(valueRect, pair.Key, int.Parse(noise.Values[pair.Key])).ToString();
                    break;
                case Noise.NoiseValueType.Float:
                    noise.Values[pair.Key] = EditorGUI.FloatField(valueRect, pair.Key, float.Parse(noise.Values[pair.Key])).ToString();
                    break;
                case Noise.NoiseValueType.String:
                    noise.Values[pair.Key] = EditorGUI.TextField(valueRect, pair.Key, noise.Values[pair.Key]);
                    break;
                case Noise.NoiseValueType.Mode:
                    noise.Values[pair.Key] = ((int)(Noise.NoiseModeType)EditorGUI.EnumPopup(valueRect, (Noise.NoiseModeType)Enum.Parse(typeof(Noise.NoiseModeType), noise.Values[pair.Key]))).ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            valueRect = new Rect(valueRect)
            {
                y = valueRect.y + EditorGUIUtility.singleLineHeight + 2
            
            };
        }

        if (EditorGUI.EndChangeCheck())
        {
            generator.RefreshAfter(index);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        generator.InitAllNoises();
        EditorGUILayout.PropertyField(mapsize, new GUIContent("Map Size"));
        noiseList.DoLayoutList();

        if (GUILayout.Button("Initialize All Noises"))
        {
            generator.InitAllNoises();
        }

        if (GUILayout.Button("Refresh All Textures"))
        {
            generator.RefreshAllTextures();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    
}