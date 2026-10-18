#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SoulboundLegends.CS.Editor
{
    [CustomEditor(typeof(CSWorldSystem))]
    public class CSWorldSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox(
                "Soulbound Legends — FINAL CS WORLD\n" +
                "Pure 5v5 elimination. Equipment and combat power come from Adventure World. " +
                "Five parent maps × seven sites each.",
                MessageType.Info);

            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();

            var world = (CSWorldSystem)target;
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Create / Reset 5 Default Parent Maps"))
            {
                Undo.RecordObject(world, "Create CS Maps");
                world.CreateDefaultMaps();
                EditorUtility.SetDirty(world);
            }

            if (GUILayout.Button("Create / Reset Shared System Connection Slots"))
            {
                Undo.RecordObject(world, "Create CS Connections");
                world.CreateDefaultConnections();
                EditorUtility.SetDirty(world);
            }

            if (GUILayout.Button("Validate Final CS Configuration"))
            {
                if (world.ValidateConfiguration(out var errors))
                    EditorUtility.DisplayDialog("CS Configuration", "Configuration is valid.", "OK");
                else
                    EditorUtility.DisplayDialog("CS Configuration Errors", string.Join("\n\n", errors), "OK");
            }
        }
    }
}
