using System;

namespace GameSystems.PersistenceSaveLoad
{
    public readonly struct PersistenceSaveLoadEntityId : IEquatable<PersistenceSaveLoadEntityId>
    {
        public readonly string Value;
        public PersistenceSaveLoadEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(PersistenceSaveLoadEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is PersistenceSaveLoadEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator PersistenceSaveLoadEntityId(string value) => new PersistenceSaveLoadEntityId(value);
    }

    public interface IPersistenceSaveLoadNetworkAdapter
    {
        void SendRequest(PersistenceSaveLoadRequest request);
        void SendResult(PersistenceSaveLoadResult result);
    }

    public interface IPersistenceSaveLoadPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
