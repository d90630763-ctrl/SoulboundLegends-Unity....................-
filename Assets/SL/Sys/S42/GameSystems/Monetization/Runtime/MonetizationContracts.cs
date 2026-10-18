using System;

namespace GameSystems.Monetization
{
    public readonly struct MonetizationEntityId : IEquatable<MonetizationEntityId>
    {
        public readonly string Value;
        public MonetizationEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(MonetizationEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MonetizationEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator MonetizationEntityId(string value) => new MonetizationEntityId(value);
    }

    public interface IMonetizationNetworkAdapter
    {
        void SendRequest(MonetizationRequest request);
        void SendResult(MonetizationResult result);
    }

    public interface IMonetizationPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
