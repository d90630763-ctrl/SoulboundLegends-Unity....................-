#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.DivineInheritanceWorldState.Editor
{
    [CustomEditor(typeof(DivineInheritanceWorldStateConfig))]
    public sealed class DivineInheritanceWorldStateConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("10 inheritances, Supreme Ruins, one-at-a-time claims, one inheritance/player, global claims, barrier and demon-war transition", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (DivineInheritanceWorldStateConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
