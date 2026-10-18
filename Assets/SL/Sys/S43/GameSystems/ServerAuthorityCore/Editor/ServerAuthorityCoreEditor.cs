#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.ServerAuthorityCore.Editor
{
    [CustomEditor(typeof(ServerAuthorityCoreConfig))]
    public sealed class ServerAuthorityCoreConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("server tick, authority, request pipeline, permissions, state replication and synchronization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (ServerAuthorityCoreConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
