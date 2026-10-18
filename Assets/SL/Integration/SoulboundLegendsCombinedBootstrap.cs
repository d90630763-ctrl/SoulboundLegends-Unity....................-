using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundLegends.Integration
{
    [CreateAssetMenu(fileName = "SoulboundLegendsCombinedConfig", menuName = "Soulbound Legends/Combined Project Config")]
    public sealed class SoulboundLegendsCombinedConfig : ScriptableObject
    {
        public string gameVersion = "1.0.0";
        public int modeUnlockLevel = 10;
        public bool enableLiveUpdateSystem = true;
        public bool enableServerAuthority = true;
        public bool enableAntiCheat = true;
        public bool enableAnalytics = true;
        public bool useExternalProductionSecrets = true;
        [TextArea(4, 12)] public string externalConfigurationNote =
            "Production endpoints, OAuth credentials, database credentials, signing keys and payment credentials must remain outside the Unity project/source archive.";
    }

    public sealed class SoulboundLegendsCombinedBootstrap : MonoBehaviour
    {
        [SerializeField] private SoulboundLegendsCombinedConfig config;
        private static SoulboundLegendsCombinedBootstrap instance;
        private readonly List<string> initializedAssemblies = new();

        public SoulboundLegendsCombinedConfig Config => config;
        public IReadOnlyList<string> InitializedAssemblies => initializedAssemblies;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeStartup()
        {
            if (FindFirstObjectByType<SoulboundLegendsCombinedBootstrap>() != null) return;
            var go = new GameObject("SoulboundLegends_CombinedBootstrap");
            instance = go.AddComponent<SoulboundLegendsCombinedBootstrap>();
            DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            DiscoverIntegratedAssemblies();
        }

        private void DiscoverIntegratedAssemblies()
        {
            initializedAssemblies.Clear();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.GetName().Name ?? string.Empty;
                if (name.Contains("Assembly-CSharp", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Soulbound", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("GameSystems", StringComparison.OrdinalIgnoreCase))
                    initializedAssemblies.Add(name);
            }
            initializedAssemblies.Sort(StringComparer.Ordinal);
        }
    }
}
