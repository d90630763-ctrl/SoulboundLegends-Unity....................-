#if UNITY_EDITOR
using UnityEditor;using UnityEngine;
namespace GameSystems.MonsterHumanGeneration.Editor{
[CustomEditor(typeof(GenerationProfile))]
public sealed class GenerationProfileEditor:UnityEditor.Editor{
 public override void OnInspectorGUI(){serializedObject.Update();DrawDefaultInspector();serializedObject.ApplyModifiedProperties();EditorGUILayout.Space(8);if(GUILayout.Button("Open Monster/Human Generator"))MonsterHumanGeneratorWindow.Open();}
}}
#endif