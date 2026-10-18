using System;

namespace GameSystems.DeathRespawn
{
    public readonly struct DeathRespawnEntityId : IEquatable<DeathRespawnEntityId>
    {
        public readonly string Value;
        public DeathRespawnEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(DeathRespawnEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is DeathRespawnEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator DeathRespawnEntityId(string value) => new DeathRespawnEntityId(value);
    }

    public interface IDeathRespawnNetworkAdapter
    {
        void SendRequest(DeathRespawnRequest request);
        void SendResult(DeathRespawnResult result);
    }

    public interface IDeathRespawnPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
