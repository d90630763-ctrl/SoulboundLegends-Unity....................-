using System;

namespace GameSystems.EconomyTrading
{
    public readonly struct EconomyTradingEntityId : IEquatable<EconomyTradingEntityId>
    {
        public readonly string Value;
        public EconomyTradingEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(EconomyTradingEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EconomyTradingEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator EconomyTradingEntityId(string value) => new EconomyTradingEntityId(value);
    }

    public interface IEconomyTradingNetworkAdapter
    {
        void SendRequest(EconomyTradingRequest request);
        void SendResult(EconomyTradingResult result);
    }

    public interface IEconomyTradingPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
