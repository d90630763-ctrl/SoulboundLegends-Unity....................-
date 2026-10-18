using System;

namespace GameSystems.DungeonBossEventInstance
{
    public readonly struct DungeonBossEventInstanceEntityId : IEquatable<DungeonBossEventInstanceEntityId>
    {
        public readonly string Value;
        public DungeonBossEventInstanceEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(DungeonBossEventInstanceEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is DungeonBossEventInstanceEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator DungeonBossEventInstanceEntityId(string value) => new DungeonBossEventInstanceEntityId(value);
    }

    public interface IDungeonBossEventInstanceNetworkAdapter
    {
        void SendRequest(DungeonBossEventInstanceRequest request);
        void SendResult(DungeonBossEventInstanceResult result);
    }

    public interface IDungeonBossEventInstancePersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
