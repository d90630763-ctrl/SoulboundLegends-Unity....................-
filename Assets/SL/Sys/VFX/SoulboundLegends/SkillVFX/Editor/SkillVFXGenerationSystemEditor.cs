#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SoulboundLegends.SkillVFX.Editor
{
    [CustomEditor(typeof(SkillVFXGenerationSystem))]
    public sealed class SkillVFXGenerationSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox(
                "Connect the Skill System, add VFX Graph templates, then create rules matching skill elements/forms. The system can automatically scale VFX from skill data.",
                MessageType.Info);
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
