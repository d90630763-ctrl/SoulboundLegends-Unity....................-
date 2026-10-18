#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.NetworkAdapterContracts.Editor
{
    [CustomEditor(typeof(NetworkAdapterContractsConfig))]
    public sealed class NetworkAdapterContractsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("network interfaces, RPC/message contracts, serialization, entity IDs and adapter abstraction", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (NetworkAdapterContractsConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
