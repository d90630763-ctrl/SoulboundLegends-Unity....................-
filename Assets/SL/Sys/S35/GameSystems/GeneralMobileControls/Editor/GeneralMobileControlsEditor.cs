#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.GeneralMobileControls.Editor
{
    [CustomEditor(typeof(GeneralMobileControlsConfig))]
    public sealed class GeneralMobileControlsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("mobile movement, attack, hand/leg controls, swipes, four-direction dodge, multitouch, sensitivity and customization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (GeneralMobileControlsConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
