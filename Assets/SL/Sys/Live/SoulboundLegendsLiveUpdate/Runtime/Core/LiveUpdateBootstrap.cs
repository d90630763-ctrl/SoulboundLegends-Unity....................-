using UnityEngine;
namespace SoulboundLegends.LiveUpdate
{
    [DefaultExecutionOrder(-10000)]
    public sealed class LiveUpdateBootstrap : MonoBehaviour
    {
        [SerializeField] private SoulboundLegendsLiveUpdateSystem systemPrefab;
        private void Awake()
        {
            if (SoulboundLegendsLiveUpdateSystem.Instance != null) return;
            if (systemPrefab != null) Instantiate(systemPrefab);
        }
    }
}
