using System;

namespace GameSystems.CharacterCombatControl
{
    public readonly struct CharacterCombatControlEntityId : IEquatable<CharacterCombatControlEntityId>
    {
        public readonly string Value;
        public CharacterCombatControlEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(CharacterCombatControlEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CharacterCombatControlEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator CharacterCombatControlEntityId(string value) => new CharacterCombatControlEntityId(value);
    }

    public interface ICharacterCombatControlNetworkAdapter
    {
        void SendRequest(CharacterCombatControlRequest request);
        void SendResult(CharacterCombatControlResult result);
    }

    public interface ICharacterCombatControlPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
