using System;

namespace GameSystems.CharacterProgression
{
    public readonly struct CharacterProgressionEntityId : IEquatable<CharacterProgressionEntityId>
    {
        public readonly string Value;
        public CharacterProgressionEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(CharacterProgressionEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CharacterProgressionEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator CharacterProgressionEntityId(string value) => new CharacterProgressionEntityId(value);
    }

    public interface ICharacterProgressionNetworkAdapter
    {
        void SendRequest(CharacterProgressionRequest request);
        void SendResult(CharacterProgressionResult result);
    }

    public interface ICharacterProgressionPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
