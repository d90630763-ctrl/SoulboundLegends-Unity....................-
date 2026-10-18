#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.AdvancedTerrainGenerator.Editor
{
    [CustomEditor(typeof(AdvancedTerrainGeneratorConfig))]
    public sealed class AdvancedTerrainGeneratorConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("description-driven fantasy terrain, mountains, valleys, cliffs, erosion-style irregular channels, branching/merging, dry channels, mesh and editor generation", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AdvancedTerrainGeneratorConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
