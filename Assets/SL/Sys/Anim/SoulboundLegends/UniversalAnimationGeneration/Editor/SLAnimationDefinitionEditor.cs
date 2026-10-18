#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace SoulboundLegends.Animation.Editor
{
    [CustomEditor(typeof(SLAnimationDefinition))] public sealed class SLAnimationDefinitionEditor:UnityEditor.Editor
    {public override void OnInspectorGUI(){serializedObject.Update();DrawDefaultInspector();serializedObject.ApplyModifiedProperties();EditorGUILayout.Space(8);if(GUILayout.Button("Open Universal Animation Director"))SLAnimationDirectorWindow.Open();EditorGUILayout.HelpBox("Describe the motion. The Director breaks complex descriptions into basic actions that can be composed and retargeted through Unity Humanoid.",MessageType.Info);}}
}
#endif
