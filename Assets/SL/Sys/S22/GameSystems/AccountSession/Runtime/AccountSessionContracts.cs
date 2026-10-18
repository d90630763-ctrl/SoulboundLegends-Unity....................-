using System;

namespace GameSystems.AccountSession
{
    public readonly struct AccountSessionEntityId : IEquatable<AccountSessionEntityId>
    {
        public readonly string Value;
        public AccountSessionEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AccountSessionEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AccountSessionEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AccountSessionEntityId(string value) => new AccountSessionEntityId(value);
    }

    public interface IAccountSessionNetworkAdapter
    {
        void SendRequest(AccountSessionRequest request);
        void SendResult(AccountSessionResult result);
    }

    public interface IAccountSessionPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
