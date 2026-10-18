#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.DynamicInnInteriorInstance.Editor
{
    [CustomEditor(typeof(DynamicInnInteriorInstanceConfig))]
    public sealed class DynamicInnInteriorInstanceConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("party-sized rooms, allocation, whole-party rerouting, entry/exit, cleanup and authority", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (DynamicInnInteriorInstanceConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
