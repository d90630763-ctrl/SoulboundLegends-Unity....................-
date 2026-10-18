using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundLegends.SupremeRuin
{
    public sealed class SupremeRuinConnectionHub : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> connectedSystems = new();

        private readonly Dictionary<SupremeRuinCapability, ISupremeRuinSystemProvider> providers = new();

        public IReadOnlyList<MonoBehaviour> ConnectedSystems => connectedSystems;

        public void Rebuild()
        {
            providers.Clear();
            foreach (var component in connectedSystems)
            {
                if (component is not ISupremeRuinSystemProvider provider || !provider.IsAvailable)
                    continue;

                foreach (var capability in provider.Capabilities)
                    if (!providers.ContainsKey(capability))
                        providers.Add(capability, provider);
            }
        }

        public bool TryGet<T>(SupremeRuinCapability capability, out T provider)
            where T : class, ISupremeRuinSystemProvider
        {
            if (providers.Count == 0) Rebuild();
            if (providers.TryGetValue(capability, out var value) && value is T typed && typed.IsAvailable)
            {
                provider = typed;
                return true;
            }
            provider = null;
            return false;
        }

        public bool Has(SupremeRuinCapability capability) => providers.ContainsKey(capability);
    }
}
