#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.LocalizationLiveOps.Editor
{
    [CustomEditor(typeof(LocalizationLiveOpsConfig))]
    public sealed class LocalizationLiveOpsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("languages, localized strings, fallback, event configuration and live content versioning", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (LocalizationLiveOpsConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
