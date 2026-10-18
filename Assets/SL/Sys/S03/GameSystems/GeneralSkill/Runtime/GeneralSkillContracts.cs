using System;

namespace GameSystems.GeneralSkill
{
    public readonly struct GeneralSkillEntityId : IEquatable<GeneralSkillEntityId>
    {
        public readonly string Value;
        public GeneralSkillEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(GeneralSkillEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is GeneralSkillEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator GeneralSkillEntityId(string value) => new GeneralSkillEntityId(value);
    }

    public interface IGeneralSkillNetworkAdapter
    {
        void SendRequest(GeneralSkillRequest request);
        void SendResult(GeneralSkillResult result);
    }

    public interface IGeneralSkillPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
