#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.AdminModeration.Editor
{
    [CustomEditor(typeof(AdminModerationConfig))]
    public sealed class AdminModerationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("permissions, admin commands, moderation, teleport, event control, ban/mute and audit logs", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AdminModerationConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
