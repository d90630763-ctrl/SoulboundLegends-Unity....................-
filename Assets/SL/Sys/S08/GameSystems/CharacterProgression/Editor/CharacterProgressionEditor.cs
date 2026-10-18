#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.CharacterProgression.Editor
{
    [CustomEditor(typeof(CharacterProgressionConfig))]
    public sealed class CharacterProgressionConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("XP, levels, ranks, health, armor, attack speed, attributes, permanent boosts, caps, rewards and authoritative progression", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (CharacterProgressionConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
