using System;

namespace GameSystems.MapNavigation
{
    public readonly struct MapNavigationEntityId : IEquatable<MapNavigationEntityId>
    {
        public readonly string Value;
        public MapNavigationEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(MapNavigationEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MapNavigationEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator MapNavigationEntityId(string value) => new MapNavigationEntityId(value);
    }

    public interface IMapNavigationNetworkAdapter
    {
        void SendRequest(MapNavigationRequest request);
        void SendResult(MapNavigationResult result);
    }

    public interface IMapNavigationPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
