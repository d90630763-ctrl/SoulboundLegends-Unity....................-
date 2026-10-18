#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.HUDSettingsSystemMenu.Editor
{
    [CustomEditor(typeof(HUDSettingsSystemMenuConfig))]
    public sealed class HUDSettingsSystemMenuConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("HUD, health/armor/stamina/skills/inventory/map hooks, system menu, settings, logout, controls, graphics/audio", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (HUDSettingsSystemMenuConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
