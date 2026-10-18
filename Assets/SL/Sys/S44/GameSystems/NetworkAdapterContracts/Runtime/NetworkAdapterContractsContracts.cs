using System;

namespace GameSystems.NetworkAdapterContracts
{
    public readonly struct NetworkAdapterContractsEntityId : IEquatable<NetworkAdapterContractsEntityId>
    {
        public readonly string Value;
        public NetworkAdapterContractsEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(NetworkAdapterContractsEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is NetworkAdapterContractsEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator NetworkAdapterContractsEntityId(string value) => new NetworkAdapterContractsEntityId(value);
    }

    public interface INetworkAdapterContractsNetworkAdapter
    {
        void SendRequest(NetworkAdapterContractsRequest request);
        void SendResult(NetworkAdapterContractsResult result);
    }

    public interface INetworkAdapterContractsPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
