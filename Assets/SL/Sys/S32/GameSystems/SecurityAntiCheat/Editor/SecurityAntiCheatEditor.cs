#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.SecurityAntiCheat.Editor
{
    [CustomEditor(typeof(SecurityAntiCheatConfig))]
    public sealed class SecurityAntiCheatConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("movement/speed/damage/inventory checks, rate limits, impossible-action detection, anomaly logging and enforcement hooks", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (SecurityAntiCheatConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
