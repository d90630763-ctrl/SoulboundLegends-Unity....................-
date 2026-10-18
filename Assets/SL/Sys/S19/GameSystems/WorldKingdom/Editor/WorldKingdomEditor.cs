#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.WorldKingdom.Editor
{
    [CustomEditor(typeof(WorldKingdomConfig))]
    public sealed class WorldKingdomConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("15 kingdoms, regions, biomes, boundaries, travel, streaming and world state", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (WorldKingdomConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
