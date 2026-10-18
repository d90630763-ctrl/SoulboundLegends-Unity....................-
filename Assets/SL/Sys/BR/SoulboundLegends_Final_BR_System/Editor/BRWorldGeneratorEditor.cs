using UnityEditor;
using UnityEngine;

namespace SoulboundLegends.BR.Editor
{
    [CustomEditor(typeof(BRWorldGenerator))]
    public sealed class BRWorldGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var generator = (BRWorldGenerator)target;

            EditorGUILayout.HelpBox(
                "Soulbound Legends BR: 5 massive fantasy maps. BR uses Adventure World weapons, skills, powers, equipment and progression. " +
                "Entry uses server-randomized anime-style teleportation with visible arrival beams. There are no CS rounds or capture objectives.",
                MessageType.Info);

            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("AUTO-CREATE ALL CONNECTION SLOTS", GUILayout.Height(32)))
            {
                Undo.RecordObject(generator, "Create BR connection slots");
                generator.AutoCreateConnectionSlots();
                EditorUtility.SetDirty(generator);
            }

            if (GUILayout.Button("VALIDATE BR CONNECTIONS", GUILayout.Height(32)))
            {
                generator.Validate(out var report);
                Debug.Log(report, generator);
                EditorUtility.DisplayDialog("BR Validation", report, "OK");
            }

            GUI.backgroundColor = new Color(0.2f, 0.8f, 1f);
            if (GUILayout.Button("GENERATE FINAL BR WORLD", GUILayout.Height(45)))
            {
                Undo.RecordObject(generator, "Generate Final BR World");
                if (generator.GenerateFinalBRWorld())
                    EditorUtility.SetDirty(generator);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox(
                "After generation, the generated BR world root is ready to be connected to the existing Lobby and server/world-loading flow. " +
                "Actual terrain/building meshes are produced by connected systems that implement IBRWorldGenerationProvider.",
                MessageType.Warning);
        }
    }
}
