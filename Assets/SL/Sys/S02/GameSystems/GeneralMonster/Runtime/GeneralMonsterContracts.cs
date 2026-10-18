using System;

namespace GameSystems.GeneralMonster
{
    public readonly struct GeneralMonsterEntityId : IEquatable<GeneralMonsterEntityId>
    {
        public readonly string Value;
        public GeneralMonsterEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(GeneralMonsterEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is GeneralMonsterEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator GeneralMonsterEntityId(string value) => new GeneralMonsterEntityId(value);
    }

    public interface IGeneralMonsterNetworkAdapter
    {
        void SendRequest(GeneralMonsterRequest request);
        void SendResult(GeneralMonsterResult result);
    }

    public interface IGeneralMonsterPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
