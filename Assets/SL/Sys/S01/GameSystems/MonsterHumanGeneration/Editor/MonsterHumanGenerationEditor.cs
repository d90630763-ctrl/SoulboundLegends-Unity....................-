#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.MonsterHumanGeneration.Editor
{
    [CustomEditor(typeof(MonsterHumanGenerationConfig))]
    public sealed class MonsterHumanGenerationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("procedural humanoid/creature mesh, descriptor parsing, textures, skeletons, IK, animation clips, prefab export, editor validation, provider contracts", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (MonsterHumanGenerationConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
