using System;

namespace GameSystems.TerrainDestruction
{
    public readonly struct TerrainDestructionEntityId : IEquatable<TerrainDestructionEntityId>
    {
        public readonly string Value;
        public TerrainDestructionEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(TerrainDestructionEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is TerrainDestructionEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator TerrainDestructionEntityId(string value) => new TerrainDestructionEntityId(value);
    }

    public interface ITerrainDestructionNetworkAdapter
    {
        void SendRequest(TerrainDestructionRequest request);
        void SendResult(TerrainDestructionResult result);
    }

    public interface ITerrainDestructionPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
