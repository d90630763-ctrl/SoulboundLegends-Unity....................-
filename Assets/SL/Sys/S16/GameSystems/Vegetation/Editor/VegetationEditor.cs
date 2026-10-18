#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.Vegetation.Editor
{
    [CustomEditor(typeof(VegetationConfig))]
    public sealed class VegetationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("grass, flowers, trees, custom models, biomes, density, wind sway, climbing, LOD, culling and pooling", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (VegetationConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
