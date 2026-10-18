using System;

namespace GameSystems.NotificationsTutorial
{
    public readonly struct NotificationsTutorialEntityId : IEquatable<NotificationsTutorialEntityId>
    {
        public readonly string Value;
        public NotificationsTutorialEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(NotificationsTutorialEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is NotificationsTutorialEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator NotificationsTutorialEntityId(string value) => new NotificationsTutorialEntityId(value);
    }

    public interface INotificationsTutorialNetworkAdapter
    {
        void SendRequest(NotificationsTutorialRequest request);
        void SendResult(NotificationsTutorialResult result);
    }

    public interface INotificationsTutorialPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
