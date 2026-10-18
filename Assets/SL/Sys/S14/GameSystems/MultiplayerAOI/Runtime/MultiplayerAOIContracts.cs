using System;

namespace GameSystems.MultiplayerAOI
{
    public readonly struct MultiplayerAOIEntityId : IEquatable<MultiplayerAOIEntityId>
    {
        public readonly string Value;
        public MultiplayerAOIEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(MultiplayerAOIEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MultiplayerAOIEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator MultiplayerAOIEntityId(string value) => new MultiplayerAOIEntityId(value);
    }

    public interface IMultiplayerAOINetworkAdapter
    {
        void SendRequest(MultiplayerAOIRequest request);
        void SendResult(MultiplayerAOIResult result);
    }

    public interface IMultiplayerAOIPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
