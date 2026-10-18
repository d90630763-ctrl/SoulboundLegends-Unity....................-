#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.EconomyTrading.Editor
{
    [CustomEditor(typeof(EconomyTradingConfig))]
    public sealed class EconomyTradingConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("currencies, balances, trades, exchange, locks, validation, anti-duplication and audit logs", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (EconomyTradingConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
