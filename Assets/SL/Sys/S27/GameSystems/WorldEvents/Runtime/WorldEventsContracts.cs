using System;

namespace GameSystems.WorldEvents
{
    public readonly struct WorldEventsEntityId : IEquatable<WorldEventsEntityId>
    {
        public readonly string Value;
        public WorldEventsEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(WorldEventsEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is WorldEventsEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator WorldEventsEntityId(string value) => new WorldEventsEntityId(value);
    }

    public interface IWorldEventsNetworkAdapter
    {
        void SendRequest(WorldEventsRequest request);
        void SendResult(WorldEventsResult result);
    }

    public interface IWorldEventsPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
