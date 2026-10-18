using UnityEngine;

namespace SoulboundLegends.Runtime
{
    /// <summary>Applies mobile-conscious quality defaults at runtime; tune after profiling target devices.</summary>
    public sealed class SLRuntimeQualityBootstrap : MonoBehaviour
    {
        [Range(30, 120)] public int targetFrameRate = 60;
        [Range(20f, 150f)] public float shadowDistance = 45f;
        [Range(.5f, 2f)] public float lodBias = 1.1f;
        public bool useTwoShadowCascades = true;
        void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = 0;
            QualitySettings.shadowDistance = shadowDistance;
            QualitySettings.shadowCascades = useTwoShadowCascades ? 2 : 0;
            QualitySettings.lodBias = lodBias;
            QualitySettings.maximumLODLevel = 0;
        }
    }
}
