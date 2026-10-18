#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.Quest.Editor
{
    [CustomEditor(typeof(QuestConfig))]
    public sealed class QuestConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("objectives, dialogue, progress, conditions, failures, rewards, chains, NPC integration and persistence", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (QuestConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
