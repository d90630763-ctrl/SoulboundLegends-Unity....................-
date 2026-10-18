using System;

namespace GameSystems.ServerAuthorityCore
{
    public readonly struct ServerAuthorityCoreEntityId : IEquatable<ServerAuthorityCoreEntityId>
    {
        public readonly string Value;
        public ServerAuthorityCoreEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(ServerAuthorityCoreEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ServerAuthorityCoreEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator ServerAuthorityCoreEntityId(string value) => new ServerAuthorityCoreEntityId(value);
    }

    public interface IServerAuthorityCoreNetworkAdapter
    {
        void SendRequest(ServerAuthorityCoreRequest request);
        void SendResult(ServerAuthorityCoreResult result);
    }

    public interface IServerAuthorityCorePersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
