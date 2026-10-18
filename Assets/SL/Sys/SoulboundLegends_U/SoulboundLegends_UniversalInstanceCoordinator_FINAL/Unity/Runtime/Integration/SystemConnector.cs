using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.InstanceCoordinator
{
    public sealed class SystemConnector : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> connectedSystems = new();
        private readonly Dictionary<string, ISystemCapabilityProvider> providers = new();

        public void Refresh()
        {
            providers.Clear();
            foreach (var component in connectedSystems)
            {
                if (component is not ISystemCapabilityProvider provider) continue;
                foreach (var capability in provider.Capabilities)
                    if (!string.IsNullOrWhiteSpace(capability)) providers[capability] = provider;
            }
        }

        public bool TryGet<T>(string capability, out T service) where T : class
        {
            if (providers.TryGetValue(capability, out var provider) && provider.GetCapability(capability) is T result)
            { service = result; return true; }
            service = null; return false;
        }
    }
}
