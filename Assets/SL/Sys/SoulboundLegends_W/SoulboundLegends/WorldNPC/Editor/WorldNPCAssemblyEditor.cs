#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SoulboundLegends.WorldNPC;

[CustomEditor(typeof(NPCSystemRegistry))]
public class NPCSystemRegistryEditor : Editor
{
    SerializedProperty systems;
    void OnEnable() => systems = serializedObject.FindProperty("connectedSystems");
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.HelpBox("Add any compatible Unity system here. The registry scans each system for capabilities and makes them available to NPCs automatically.", MessageType.Info);
        EditorGUILayout.PropertyField(systems, new GUIContent("Connected Systems (+ Add System)"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scanChildrenAutomatically"));
        if (GUILayout.Button("Scan / Rebuild Capability Map")) ((NPCSystemRegistry)target).RebuildCapabilityMap();
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(WorldNPCAssemblyGenerator))]
public class WorldNPCAssemblyGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.HelpBox("General NPC generator. Gender is automatically generated; there is intentionally no Male/Female generator button. Voice category follows generated gender and the connected Voice System supplies the individual voice.", MessageType.Info);
        DrawDefaultInspector();
        var gen = (WorldNPCAssemblyGenerator)target;
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("GENERATION", EditorStyles.boldLabel);
        if (GUILayout.Button("GENERATE PRIMARY NPCs")) gen.GeneratePrimaryNPCs();
        if (GUILayout.Button("GENERATE SECONDARY NPCs")) gen.GenerateSecondaryNPCs();
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(NPCController))]
public class NPCControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.HelpBox("NPC identity, routine, navigation, interaction, voice and services are assembled here. Services are filtered by the occupation definition and server authorization at runtime.", MessageType.Info);
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
