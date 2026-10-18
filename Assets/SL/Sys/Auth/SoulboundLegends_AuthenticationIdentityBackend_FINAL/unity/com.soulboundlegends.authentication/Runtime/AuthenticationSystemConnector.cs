using System;
using UnityEngine;

namespace SoulboundLegends.Authentication
{
    public interface ISoulboundSystemConnector
    {
        string SystemName { get; }
        void Register(object systemInstance);
        void Unregister(object systemInstance);
    }

    public sealed class AuthenticationSystemConnections : MonoBehaviour, ISoulboundSystemConnector
    {
        [Serializable] public sealed class Connection
        {
            public string Capability;
            [Tooltip("Assign any compatible Soulbound Legends system component. Authentication does not hard-code its concrete type.")]
            public MonoBehaviour System;
        }

        [SerializeField] private Connection[] connections = Array.Empty<Connection>();

        public string SystemName => "Authentication & Identity Backend";

        public void Register(object systemInstance) { }
        public void Unregister(object systemInstance) { }

        public T FindCapability<T>(string capability) where T : class
        {
            foreach (var c in connections)
            {
                if (!string.Equals(c.Capability, capability, StringComparison.OrdinalIgnoreCase)) continue;
                if (c.System is T typed) return typed;
            }
            return null;
        }
    }
}
