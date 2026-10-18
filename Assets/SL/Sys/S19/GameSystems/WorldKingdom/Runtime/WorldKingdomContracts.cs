using System;

namespace GameSystems.WorldKingdom
{
    public readonly struct WorldKingdomEntityId : IEquatable<WorldKingdomEntityId>
    {
        public readonly string Value;
        public WorldKingdomEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(WorldKingdomEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is WorldKingdomEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator WorldKingdomEntityId(string value) => new WorldKingdomEntityId(value);
    }

    public interface IWorldKingdomNetworkAdapter
    {
        void SendRequest(WorldKingdomRequest request);
        void SendResult(WorldKingdomResult result);
    }

    public interface IWorldKingdomPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
