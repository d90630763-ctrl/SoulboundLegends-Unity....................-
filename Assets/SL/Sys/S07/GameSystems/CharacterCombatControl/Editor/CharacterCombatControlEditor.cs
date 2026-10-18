#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.CharacterCombatControl.Editor
{
    [CustomEditor(typeof(CharacterCombatControlConfig))]
    public sealed class CharacterCombatControlConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("mobile combat movement, limb controls, swipe direction, tap thrust, defense, dodge, targeting, stamina, cooldowns, IK and authority", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (CharacterCombatControlConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
