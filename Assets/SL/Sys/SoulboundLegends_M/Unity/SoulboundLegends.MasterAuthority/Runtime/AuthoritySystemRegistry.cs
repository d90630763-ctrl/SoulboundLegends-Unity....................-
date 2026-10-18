using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Authority
{
    [Serializable]
    public sealed class AuthoritySystemSlot
    {
        [Tooltip("Drag the system component here. It must implement IAuthoritySystem or IAuthorityRequestSource.")]
        public MonoBehaviour system;
    }

    public sealed class AuthoritySystemRegistry : MonoBehaviour
    {
        [Header("CENTRAL AUTHORITY CONNECTIONS")]
        [Tooltip("Add ANY future system here. The Master Authority will attach to it and make the server the authorization gate.")]
        [SerializeField] private List<AuthoritySystemSlot> connectedSystems = new();

        [Header("Connection Rules")]
        [SerializeField] private bool autoDiscoverChildren = true;
        [SerializeField] private bool rejectDuplicateSystemIds = true;
        [SerializeField] private bool requireAuthorityBeforeSystemUse = true;

        private readonly List<IAuthoritySystem> systems = new();
        private MasterAuthorityClient authority;
        private readonly HashSet<string> systemIds = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<IAuthoritySystem> Systems => systems;
        public bool AuthorityAttached => authority != null;

        public void Initialize(MasterAuthorityClient masterAuthority)
        {
            authority = masterAuthority;
            Rebuild();
            foreach (var system in systems)
                system.OnAuthorityReady(authority);

            foreach (var slot in connectedSystems)
            {
                if (slot?.system is IAuthorityRequestSource source)
                    source.AttachAuthority(authority);
            }
        }

        public void Rebuild()
        {
            systems.Clear();
            systemIds.Clear();

            if (autoDiscoverChildren)
            {
                foreach (var component in GetComponentsInChildren<MonoBehaviour>(true))
                    TryAdd(component);
            }

            foreach (var slot in connectedSystems)
                TryAdd(slot?.system);
        }

        private void TryAdd(MonoBehaviour component)
        {
            if (component == null || component == this) return;
            if (component is not IAuthoritySystem system) return;
            if (string.IsNullOrWhiteSpace(system.SystemId)) return;

            if (rejectDuplicateSystemIds && !systemIds.Add(system.SystemId))
            {
                Debug.LogError($"Master Authority: duplicate SystemId '{system.SystemId}'. Only one connected instance is allowed.", component);
                return;
            }

            if (!systems.Contains(system)) systems.Add(system);
        }

        public bool IsSystemConnected(string systemId)
        {
            foreach (var system in systems)
                if (string.Equals(system.SystemId, systemId, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static AuthoritySystemRegistry FindInScene()
            => FindFirstObjectByType<AuthoritySystemRegistry>();
    }
}