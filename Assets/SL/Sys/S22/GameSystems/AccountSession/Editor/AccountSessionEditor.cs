#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.AccountSession.Editor
{
    [CustomEditor(typeof(AccountSessionConfig))]
    public sealed class AccountSessionConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("account identity, auth hooks, sessions, logout, reconnect, timeout and duplicate-session prevention", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AccountSessionConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
