using System;

namespace GameSystems.AdvancedTerrainGenerator
{
    public readonly struct AdvancedTerrainGeneratorEntityId : IEquatable<AdvancedTerrainGeneratorEntityId>
    {
        public readonly string Value;
        public AdvancedTerrainGeneratorEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AdvancedTerrainGeneratorEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AdvancedTerrainGeneratorEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AdvancedTerrainGeneratorEntityId(string value) => new AdvancedTerrainGeneratorEntityId(value);
    }

    public interface IAdvancedTerrainGeneratorNetworkAdapter
    {
        void SendRequest(AdvancedTerrainGeneratorRequest request);
        void SendResult(AdvancedTerrainGeneratorResult result);
    }

    public interface IAdvancedTerrainGeneratorPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
