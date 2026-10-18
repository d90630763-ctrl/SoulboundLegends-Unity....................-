#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.MatchmakingSessions.Editor
{
    [CustomEditor(typeof(MatchmakingSessionsConfig))]
    public sealed class MatchmakingSessionsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("queues, party matching, sessions, limits, ready/cancel/reconnect and assignment", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (MatchmakingSessionsConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
