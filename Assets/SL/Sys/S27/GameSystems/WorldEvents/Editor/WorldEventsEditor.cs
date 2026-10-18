#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.WorldEvents.Editor
{
    [CustomEditor(typeof(WorldEventsConfig))]
    public sealed class WorldEventsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("timers, conditions, spawning, world bosses, environmental events, participation, rewards and reset", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (WorldEventsConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
