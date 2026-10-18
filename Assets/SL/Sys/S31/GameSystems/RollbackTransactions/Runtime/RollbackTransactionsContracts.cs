using System;

namespace GameSystems.RollbackTransactions
{
    public readonly struct RollbackTransactionsEntityId : IEquatable<RollbackTransactionsEntityId>
    {
        public readonly string Value;
        public RollbackTransactionsEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(RollbackTransactionsEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is RollbackTransactionsEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator RollbackTransactionsEntityId(string value) => new RollbackTransactionsEntityId(value);
    }

    public interface IRollbackTransactionsNetworkAdapter
    {
        void SendRequest(RollbackTransactionsRequest request);
        void SendResult(RollbackTransactionsResult result);
    }

    public interface IRollbackTransactionsPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
