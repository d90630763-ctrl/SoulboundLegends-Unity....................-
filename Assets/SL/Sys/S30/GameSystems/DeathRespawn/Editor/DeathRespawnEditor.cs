#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.DeathRespawn.Editor
{
    [CustomEditor(typeof(DeathRespawnConfig))]
    public sealed class DeathRespawnConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("death, respawn, revive, timers, spawn points, party revive, penalties, loot rules and authority", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (DeathRespawnConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
