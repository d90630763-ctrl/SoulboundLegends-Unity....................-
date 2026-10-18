#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.GeneralSkill.Editor
{
    [CustomEditor(typeof(GeneralSkillConfig))]
    public sealed class GeneralSkillConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("attack/heal/movement/utility skills, damage/healing, buffs/debuffs, elements, forms, costs, cooldowns, scaling, destruction and VFX/SFX hooks", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (GeneralSkillConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
