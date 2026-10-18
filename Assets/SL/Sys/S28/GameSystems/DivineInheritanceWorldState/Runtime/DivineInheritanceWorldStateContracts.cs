using System;

namespace GameSystems.DivineInheritanceWorldState
{
    public readonly struct DivineInheritanceWorldStateEntityId : IEquatable<DivineInheritanceWorldStateEntityId>
    {
        public readonly string Value;
        public DivineInheritanceWorldStateEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(DivineInheritanceWorldStateEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is DivineInheritanceWorldStateEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator DivineInheritanceWorldStateEntityId(string value) => new DivineInheritanceWorldStateEntityId(value);
    }

    public interface IDivineInheritanceWorldStateNetworkAdapter
    {
        void SendRequest(DivineInheritanceWorldStateRequest request);
        void SendResult(DivineInheritanceWorldStateResult result);
    }

    public interface IDivineInheritanceWorldStatePersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
