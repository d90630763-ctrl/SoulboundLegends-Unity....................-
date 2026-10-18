#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.GameFoundationBootstrap.Editor
{
    [CustomEditor(typeof(GameFoundationBootstrapConfig))]
    public sealed class GameFoundationBootstrapConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("initialization, dependency registration, config loading, persistent managers, lifecycle and shutdown", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (GameFoundationBootstrapConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
