using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    /// <summary>
    /// Development/test authority only. Never use this component as the production
    /// multiplayer authority. Production must connect a real server authority provider.
    /// </summary>
    public sealed class RiftLocalTestAuthority : MonoBehaviour, IRiftServerAuthorityProvider
    {
        [SerializeField] private int priority = -1000;
        private readonly Dictionary<string, HashSet<string>> nonces = new();

        public string ProviderId => name;
        public int Priority => priority;
        public IReadOnlyCollection<RiftCapability> Capabilities => new[] { RiftCapability.ServerAuthority };
        public bool IsAvailable => isActiveAndEnabled;
        public bool IsAuthoritativeServer => true;
        public long ServerUnixMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public bool ValidateRequestNonce(string playerId, string nonce)
        {
            if (string.IsNullOrWhiteSpace(playerId) || string.IsNullOrWhiteSpace(nonce))
                return false;

            if (!nonces.TryGetValue(playerId, out var set))
            {
                set = new HashSet<string>();
                nonces[playerId] = set;
            }

            return set.Add(nonce);
        }

        public bool ConsumeRateLimit(string playerId, string action, int cost) => true;
    }
}
