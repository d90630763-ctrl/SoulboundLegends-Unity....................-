using System;

namespace GameSystems.MatchmakingSessions
{
    public readonly struct MatchmakingSessionsEntityId : IEquatable<MatchmakingSessionsEntityId>
    {
        public readonly string Value;
        public MatchmakingSessionsEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(MatchmakingSessionsEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MatchmakingSessionsEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator MatchmakingSessionsEntityId(string value) => new MatchmakingSessionsEntityId(value);
    }

    public interface IMatchmakingSessionsNetworkAdapter
    {
        void SendRequest(MatchmakingSessionsRequest request);
        void SendResult(MatchmakingSessionsResult result);
    }

    public interface IMatchmakingSessionsPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
