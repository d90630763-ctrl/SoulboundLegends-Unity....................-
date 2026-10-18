using System;

namespace GameSystems.AdminModeration
{
    public readonly struct AdminModerationEntityId : IEquatable<AdminModerationEntityId>
    {
        public readonly string Value;
        public AdminModerationEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AdminModerationEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AdminModerationEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AdminModerationEntityId(string value) => new AdminModerationEntityId(value);
    }

    public interface IAdminModerationNetworkAdapter
    {
        void SendRequest(AdminModerationRequest request);
        void SendResult(AdminModerationResult result);
    }

    public interface IAdminModerationPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
