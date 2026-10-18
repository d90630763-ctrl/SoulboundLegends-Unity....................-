#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.ServerAuthorizationSecurity.Editor
{
    [CustomEditor(typeof(ServerAuthorizationSecurityConfig))]
    public sealed class ServerAuthorizationSecurityConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("request validation, identity, permission, distance, cooldown, resources, ownership, rate limits, nonces and rejection handling", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (ServerAuthorizationSecurityConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
