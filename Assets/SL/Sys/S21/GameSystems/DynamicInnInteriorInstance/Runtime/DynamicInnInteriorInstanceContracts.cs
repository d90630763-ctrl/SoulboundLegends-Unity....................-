using System;

namespace GameSystems.DynamicInnInteriorInstance
{
    public readonly struct DynamicInnInteriorInstanceEntityId : IEquatable<DynamicInnInteriorInstanceEntityId>
    {
        public readonly string Value;
        public DynamicInnInteriorInstanceEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(DynamicInnInteriorInstanceEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is DynamicInnInteriorInstanceEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator DynamicInnInteriorInstanceEntityId(string value) => new DynamicInnInteriorInstanceEntityId(value);
    }

    public interface IDynamicInnInteriorInstanceNetworkAdapter
    {
        void SendRequest(DynamicInnInteriorInstanceRequest request);
        void SendResult(DynamicInnInteriorInstanceResult result);
    }

    public interface IDynamicInnInteriorInstancePersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
