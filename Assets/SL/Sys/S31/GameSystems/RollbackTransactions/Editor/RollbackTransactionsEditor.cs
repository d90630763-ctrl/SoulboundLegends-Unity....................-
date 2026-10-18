#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.RollbackTransactions.Editor
{
    [CustomEditor(typeof(RollbackTransactionsConfig))]
    public sealed class RollbackTransactionsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("transaction IDs, atomic commit/rollback, inventory/currency/crafting recovery and disconnect recovery", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (RollbackTransactionsConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
