#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameSystems.PersistenceSaveLoad.Editor
{
    [CustomEditor(typeof(PersistenceSaveLoadConfig))]
    public sealed class PersistenceSaveLoadConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("profiles, inventory, equipment, progression, quests, world state, timestamps, versioning, migration and safe saving", MessageType.Info);
            if (GUILayout.Button("Validate Configuration"))
            {
                var c = (PersistenceSaveLoadConfig)target;
                if (string.IsNullOrWhiteSpace(c.systemId))
                    EditorUtility.DisplayDialog("Validation", "System ID is required.", "OK");
                else
                    EditorUtility.DisplayDialog("Validation", "Configuration passed basic validation.", "OK");
            }
        }
    }
}
#endif
