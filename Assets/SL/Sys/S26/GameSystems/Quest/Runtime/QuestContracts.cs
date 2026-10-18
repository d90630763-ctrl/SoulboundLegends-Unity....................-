using System;

namespace GameSystems.Quest
{
    public readonly struct QuestEntityId : IEquatable<QuestEntityId>
    {
        public readonly string Value;
        public QuestEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(QuestEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is QuestEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator QuestEntityId(string value) => new QuestEntityId(value);
    }

    public interface IQuestNetworkAdapter
    {
        void SendRequest(QuestRequest request);
        void SendResult(QuestResult result);
    }

    public interface IQuestPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
