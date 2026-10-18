using System;

namespace GameSystems.Alchemy
{
    public readonly struct AlchemyEntityId : IEquatable<AlchemyEntityId>
    {
        public readonly string Value;
        public AlchemyEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AlchemyEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AlchemyEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AlchemyEntityId(string value) => new AlchemyEntityId(value);
    }

    public interface IAlchemyNetworkAdapter
    {
        void SendRequest(AlchemyRequest request);
        void SendResult(AlchemyResult result);
    }

    public interface IAlchemyPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
