#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.Monetization.Editor
{
    [CustomEditor(typeof(MonetizationConfig))]
    public sealed class MonetizationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("products, purchases, receipts, entitlements, premium currency/items, server verification and anti-duplication", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (MonetizationConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
