#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.GeneralMonster.Editor
{
    [CustomEditor(typeof(GeneralMonsterConfig))]
    public sealed class GeneralMonsterConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("monster AI, aggro, distance behavior, flying targets, traits, elements, resistances, health, damage, loot, spawn/despawn, authority hooks", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (GeneralMonsterConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
