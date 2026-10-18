#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SoulboundLegends.Voice;

[CustomEditor(typeof(VoiceTransformationSystem))]
public class VoiceTransformationSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(
            "Soulbound Legends Voice Transformation System. " +
            "Add this component to one GameObject. The in-game UI can be generated automatically.",
            MessageType.Info);

        DrawDefaultInspector();

        GUILayout.Space(8);

        VoiceTransformationSystem system = (VoiceTransformationSystem)target;

        if (GUILayout.Button("Build In-Game UI"))
        {
            system.BuildUI();
            EditorUtility.SetDirty(system);
        }

        if (GUILayout.Button("Apply Current Settings"))
        {
            system.ApplySettings();
            EditorUtility.SetDirty(system);
        }

        if (GUILayout.Button("Save Voice Settings"))
        {
            system.SaveSettings();
            EditorUtility.SetDirty(system);
        }
    }
}
#endif
