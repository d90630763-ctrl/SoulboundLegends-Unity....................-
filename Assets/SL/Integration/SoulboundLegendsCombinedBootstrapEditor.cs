#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace SoulboundLegends.Integration
{
    public static class SoulboundLegendsCombinedMenu
    {
        [MenuItem("Soulbound Legends/Integration/Create Combined Bootstrap")]
        public static void CreateBootstrap()
        {
            var go = new GameObject("SoulboundLegends_CombinedBootstrap");
            go.AddComponent<SoulboundLegendsCombinedBootstrap>();
            Selection.activeGameObject = go;
            EditorUtility.SetDirty(go);
        }
    }
}
#endif
