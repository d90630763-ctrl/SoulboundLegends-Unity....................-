#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.OptimizationDevice.Editor
{
    [CustomEditor(typeof(OptimizationDeviceConfig))]
    public sealed class OptimizationDeviceConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("quality profiles, LOD, culling, pooling, resolution scaling, memory management, FPS targets and device detection", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (OptimizationDeviceConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
