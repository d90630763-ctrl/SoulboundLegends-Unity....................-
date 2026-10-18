#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.Portal.Editor
{
    [CustomEditor(typeof(PortalConfig))]
    public sealed class PortalConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("kingdom portals, aether payment, destinations, party travel, spatial tears, boss portals, conditions and authorization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (PortalConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
