#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.MultiplayerAOI.Editor
{
    [CustomEditor(typeof(MultiplayerAOIConfig))]
    public sealed class MultiplayerAOIConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("grid AOI, dynamic movement, enter/exit, visibility cap, synchronization and optimization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (MultiplayerAOIConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
