using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftSystemConnectionHub : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> connectedSystems = new();

        private readonly Dictionary<RiftCapability, List<IRiftSystemProvider>> providers = new();

        public IReadOnlyList<MonoBehaviour> ConnectedSystems => connectedSystems;

        public void Rebuild()
        {
            providers.Clear();
            foreach (var component in connectedSystems)
            {
                if (component is not IRiftSystemProvider provider || !provider.IsAvailable)
                    continue;

                foreach (var capability in provider.Capabilities)
                {
                    if (!providers.TryGetValue(capability, out var list))
                    {
                        list = new List<IRiftSystemProvider>();
                        providers.Add(capability, list);
                    }
                    if (!list.Contains(provider))
                        list.Add(provider);
                }
            }

            foreach (var pair in providers)
                pair.Value.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public bool TryGet<T>(RiftCapability capability, out T provider) where T : class, IRiftSystemProvider
        {
            if (providers.TryGetValue(capability, out var list))
            {
                foreach (var candidate in list)
                {
                    if (candidate is T typed && candidate.IsAvailable)
                    {
                        provider = typed;
                        return true;
                    }
                }
            }

            provider = null;
            return false;
        }

        public IReadOnlyList<T> GetAll<T>(RiftCapability capability) where T : class, IRiftSystemProvider
        {
            if (!providers.TryGetValue(capability, out var list))
                return Array.Empty<T>();
            return list.OfType<T>().Where(x => x.IsAvailable).ToArray();
        }

        public bool Has(RiftCapability capability) =>
            providers.TryGetValue(capability, out var list) && list.Any(x => x.IsAvailable);
    }
}
