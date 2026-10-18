using System;

namespace GameSystems.GameFoundationBootstrap
{
    public readonly struct GameFoundationBootstrapEntityId : IEquatable<GameFoundationBootstrapEntityId>
    {
        public readonly string Value;
        public GameFoundationBootstrapEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(GameFoundationBootstrapEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is GameFoundationBootstrapEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator GameFoundationBootstrapEntityId(string value) => new GameFoundationBootstrapEntityId(value);
    }

    public interface IGameFoundationBootstrapNetworkAdapter
    {
        void SendRequest(GameFoundationBootstrapRequest request);
        void SendResult(GameFoundationBootstrapResult result);
    }

    public interface IGameFoundationBootstrapPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
