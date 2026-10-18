using System;

namespace GameSystems.NPC
{
    public readonly struct NPCEntityId : IEquatable<NPCEntityId>
    {
        public readonly string Value;
        public NPCEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(NPCEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is NPCEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator NPCEntityId(string value) => new NPCEntityId(value);
    }

    public interface INPCNetworkAdapter
    {
        void SendRequest(NPCRequest request);
        void SendResult(NPCResult result);
    }

    public interface INPCPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
