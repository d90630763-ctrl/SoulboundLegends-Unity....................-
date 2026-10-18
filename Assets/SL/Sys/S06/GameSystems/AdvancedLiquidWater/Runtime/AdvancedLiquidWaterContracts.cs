using System;

namespace GameSystems.AdvancedLiquidWater
{
    public readonly struct AdvancedLiquidWaterEntityId : IEquatable<AdvancedLiquidWaterEntityId>
    {
        public readonly string Value;
        public AdvancedLiquidWaterEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AdvancedLiquidWaterEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AdvancedLiquidWaterEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AdvancedLiquidWaterEntityId(string value) => new AdvancedLiquidWaterEntityId(value);
    }

    public interface IAdvancedLiquidWaterNetworkAdapter
    {
        void SendRequest(AdvancedLiquidWaterRequest request);
        void SendResult(AdvancedLiquidWaterResult result);
    }

    public interface IAdvancedLiquidWaterPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
