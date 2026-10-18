#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SoulboundLegends.Progression;

public static class SBLProgressionMenu
{
    [MenuItem("Soulbound Legends/Progression/Create Config")]
    public static void CreateConfig()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Progression Config", "SBL_ProgressionConfig", "asset",
            "Choose where to save the progression configuration.");
        if (string.IsNullOrEmpty(path)) return;

        var config = ScriptableObject.CreateInstance<SBLProgressionConfig>();
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = config;
    }

    [MenuItem("Soulbound Legends/Progression/Create System Manager")]
    public static void CreateSystemManager()
    {
        GameObject go = new GameObject("SBL Progression System");
        go.AddComponent<SBLProgressionService>();
        Selection.activeGameObject = go;
        Undo.RegisterCreatedObjectUndo(go, "Create SBL Progression System");
    }
}
#endif
