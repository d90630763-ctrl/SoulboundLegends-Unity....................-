using System;

namespace GameSystems.ServerAuthorizationSecurity
{
    public readonly struct ServerAuthorizationSecurityEntityId : IEquatable<ServerAuthorizationSecurityEntityId>
    {
        public readonly string Value;
        public ServerAuthorizationSecurityEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(ServerAuthorizationSecurityEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ServerAuthorizationSecurityEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator ServerAuthorizationSecurityEntityId(string value) => new ServerAuthorizationSecurityEntityId(value);
    }

    public interface IServerAuthorizationSecurityNetworkAdapter
    {
        void SendRequest(ServerAuthorizationSecurityRequest request);
        void SendResult(ServerAuthorizationSecurityResult result);
    }

    public interface IServerAuthorizationSecurityPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
