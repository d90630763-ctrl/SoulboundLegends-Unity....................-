using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public class NPCSystemRegistry : MonoBehaviour
    {
        [Tooltip("Add any MonoBehaviour-based system here. The NPC system inspects its capabilities automatically.")]
        [SerializeField] private List<MonoBehaviour> connectedSystems = new();
        [SerializeField] private bool scanChildrenAutomatically = true;
        private readonly Dictionary<NPCSystemCapability, List<Object>> capabilityMap = new();

        public IReadOnlyList<MonoBehaviour> ConnectedSystems => connectedSystems;

        private void Awake() => RebuildCapabilityMap();
        private void OnValidate() { if (!Application.isPlaying) RebuildCapabilityMap(); }

        public void RebuildCapabilityMap()
        {
            capabilityMap.Clear();
            var all = new List<MonoBehaviour>(connectedSystems);
            if (scanChildrenAutomatically)
            {
                foreach (var c in GetComponentsInChildren<MonoBehaviour>(true)) if (!all.Contains(c)) all.Add(c);
            }
            foreach (var system in all)
            {
                if (!system) continue;
                if (system is INPCSystemCapabilityProvider provider)
                {
                    var caps = new List<NPCSystemCapability>(); provider.GetCapabilities(caps);
                    foreach (var cap in caps) AddCapability(cap, system);
                }
            }
        }

        private void AddCapability(NPCSystemCapability cap, Object obj)
        {
            if (!capabilityMap.TryGetValue(cap, out var list)) capabilityMap[cap] = list = new List<Object>();
            if (!list.Contains(obj)) list.Add(obj);
        }

        public T Find<T>() where T : class
        {
            foreach (var s in connectedSystems) if (s is T t) return t;
            foreach (var s in GetComponentsInChildren<MonoBehaviour>(true)) if (s is T t) return t;
            return null;
        }

        public bool HasCapability(NPCSystemCapability capability) => capabilityMap.ContainsKey(capability) && capabilityMap[capability].Count > 0;
        public List<Object> GetProviders(NPCSystemCapability capability) => capabilityMap.TryGetValue(capability, out var list) ? new List<Object>(list) : new List<Object>();

        public void AddSystem(MonoBehaviour system) { if (system && !connectedSystems.Contains(system)) { connectedSystems.Add(system); RebuildCapabilityMap(); } }
        public void RemoveSystem(MonoBehaviour system) { if (connectedSystems.Remove(system)) RebuildCapabilityMap(); }
    }
}
