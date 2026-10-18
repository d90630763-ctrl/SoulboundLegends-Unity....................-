#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SoulboundLegends.Authority;

[CustomEditor(typeof(AuthoritySystemRegistry))]
public sealed class AuthoritySystemRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.HelpBox("Every system placed in Connected Systems is forced through the Master Authority gateway. Add future systems with the + button.", MessageType.Info);
        DrawPropertiesExcluding(serializedObject, "m_Script");
        serializedObject.ApplyModifiedProperties();
    }
}
#endif