#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.PartyArtifactLink.Editor
{
    [CustomEditor(typeof(PartyArtifactLinkConfig))]
    public sealed class PartyArtifactLinkConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("party links, anywhere activation, shared damage, distribution, cooldown, disconnect/death handling and authority", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (PartyArtifactLinkConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
