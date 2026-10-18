#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.AnimationIK.Editor
{
    [CustomEditor(typeof(AnimationIKConfig))]
    public sealed class AnimationIKConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("humanoid/creature mapping, IK, hand/foot placement, limb targets, blending, constraints, events and synchronization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AnimationIKConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
