#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.Alchemy.Editor
{
    [CustomEditor(typeof(AlchemyConfig))]
    public sealed class AlchemyConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("alchemy recipes, material inputs, pills/liquids, colors/shapes/effects, output slots and transaction authorization", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (AlchemyConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
