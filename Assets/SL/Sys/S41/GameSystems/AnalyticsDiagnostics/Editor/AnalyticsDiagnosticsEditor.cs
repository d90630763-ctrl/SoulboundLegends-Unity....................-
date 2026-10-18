#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.AnalyticsDiagnostics.Editor
{
    [CustomEditor(typeof(AnalyticsDiagnosticsConfig))]
    public sealed class AnalyticsDiagnosticsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("errors, performance, FPS/network/server diagnostics, telemetry hooks and log management", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AnalyticsDiagnosticsConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
