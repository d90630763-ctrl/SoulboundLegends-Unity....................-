#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
namespace SoulboundLegends.LiveUpdate.Editor
{
    public static class LiveUpdateManifestGenerator
    {
        [MenuItem("Soulbound Legends/Live Update/Create Example Manifest")]
        public static void Create()
        {
            string dir = "Assets/StreamingAssets/SoulboundLegendsLiveUpdates/production"; Directory.CreateDirectory(dir);
            var m = new LiveUpdateManifest { manifestId = "initial-" + System.DateTime.UtcNow.Ticks, contentVersion = "1.0.1", stage = UpdateReleaseStage.Staged, applyMode = UpdateApplyMode.HotApply, configJson = "{\"flags\":[]}" };
            File.WriteAllText(Path.Combine(dir, "manifest.json"), JsonUtility.ToJson(new UpdateManifestEnvelope { manifest = m }, true)); AssetDatabase.Refresh(); EditorUtility.DisplayDialog("Soulbound Legends", "Example manifest created.", "OK");
        }
        [MenuItem("Soulbound Legends/Live Update/Open Persistent Data Folder")]
        public static void OpenData() => EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
#endif
