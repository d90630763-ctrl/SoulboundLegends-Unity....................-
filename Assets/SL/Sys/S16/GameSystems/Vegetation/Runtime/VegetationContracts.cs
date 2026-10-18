using System;

namespace GameSystems.Vegetation
{
    public readonly struct VegetationEntityId : IEquatable<VegetationEntityId>
    {
        public readonly string Value;
        public VegetationEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(VegetationEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is VegetationEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator VegetationEntityId(string value) => new VegetationEntityId(value);
    }

    public interface IVegetationNetworkAdapter
    {
        void SendRequest(VegetationRequest request);
        void SendResult(VegetationResult result);
    }

    public interface IVegetationPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
