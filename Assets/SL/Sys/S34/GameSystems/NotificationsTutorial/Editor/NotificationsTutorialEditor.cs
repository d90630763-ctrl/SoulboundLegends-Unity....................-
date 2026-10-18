#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.NotificationsTutorial.Editor
{
    [CustomEditor(typeof(NotificationsTutorialConfig))]
    public sealed class NotificationsTutorialConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("tutorials, prompts, notifications, objectives, rewards, system messages, persistence and localization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (NotificationsTutorialConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
