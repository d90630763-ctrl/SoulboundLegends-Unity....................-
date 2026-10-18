using System;

namespace GameSystems.SecurityAntiCheat
{
    public readonly struct SecurityAntiCheatEntityId : IEquatable<SecurityAntiCheatEntityId>
    {
        public readonly string Value;
        public SecurityAntiCheatEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(SecurityAntiCheatEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is SecurityAntiCheatEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator SecurityAntiCheatEntityId(string value) => new SecurityAntiCheatEntityId(value);
    }

    public interface ISecurityAntiCheatNetworkAdapter
    {
        void SendRequest(SecurityAntiCheatRequest request);
        void SendResult(SecurityAntiCheatResult result);
    }

    public interface ISecurityAntiCheatPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
