using System;

namespace GameSystems.SocialFriendsChat
{
    public readonly struct SocialFriendsChatEntityId : IEquatable<SocialFriendsChatEntityId>
    {
        public readonly string Value;
        public SocialFriendsChatEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(SocialFriendsChatEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is SocialFriendsChatEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator SocialFriendsChatEntityId(string value) => new SocialFriendsChatEntityId(value);
    }

    public interface ISocialFriendsChatNetworkAdapter
    {
        void SendRequest(SocialFriendsChatRequest request);
        void SendResult(SocialFriendsChatResult result);
    }

    public interface ISocialFriendsChatPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
