#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.DungeonBossEventInstance.Editor
{
    [CustomEditor(typeof(DungeonBossEventInstanceConfig))]
    public sealed class DungeonBossEventInstanceConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("solo/party instances, player limits, bosses, phases, difficulty, training, timers, rewards, reset and cleanup", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (DungeonBossEventInstanceConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
