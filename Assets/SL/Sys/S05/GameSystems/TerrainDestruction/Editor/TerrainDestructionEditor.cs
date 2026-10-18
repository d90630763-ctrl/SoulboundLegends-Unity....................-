#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.TerrainDestruction.Editor
{
    [CustomEditor(typeof(TerrainDestructionConfig))]
    public sealed class TerrainDestructionConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("server-validated craters, holes, cuts, slashes, explosions, meteor deformation, debris and persistent deformation", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (TerrainDestructionConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
