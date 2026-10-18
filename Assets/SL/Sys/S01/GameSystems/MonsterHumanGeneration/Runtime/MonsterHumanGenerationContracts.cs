using System;

namespace GameSystems.MonsterHumanGeneration
{
    public readonly struct MonsterHumanGenerationEntityId : IEquatable<MonsterHumanGenerationEntityId>
    {
        public readonly string Value;
        public MonsterHumanGenerationEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(MonsterHumanGenerationEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MonsterHumanGenerationEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator MonsterHumanGenerationEntityId(string value) => new MonsterHumanGenerationEntityId(value);
    }

    public interface IMonsterHumanGenerationNetworkAdapter
    {
        void SendRequest(MonsterHumanGenerationRequest request);
        void SendResult(MonsterHumanGenerationResult result);
    }

    public interface IMonsterHumanGenerationPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
