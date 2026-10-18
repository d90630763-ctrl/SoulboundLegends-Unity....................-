using System;

namespace GameSystems.Portal
{
    public readonly struct PortalEntityId : IEquatable<PortalEntityId>
    {
        public readonly string Value;
        public PortalEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(PortalEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is PortalEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator PortalEntityId(string value) => new PortalEntityId(value);
    }

    public interface IPortalNetworkAdapter
    {
        void SendRequest(PortalRequest request);
        void SendResult(PortalResult result);
    }

    public interface IPortalPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
