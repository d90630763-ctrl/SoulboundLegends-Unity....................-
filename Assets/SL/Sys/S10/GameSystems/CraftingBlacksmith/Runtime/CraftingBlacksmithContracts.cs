using System;

namespace GameSystems.CraftingBlacksmith
{
    public readonly struct CraftingBlacksmithEntityId : IEquatable<CraftingBlacksmithEntityId>
    {
        public readonly string Value;
        public CraftingBlacksmithEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(CraftingBlacksmithEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CraftingBlacksmithEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator CraftingBlacksmithEntityId(string value) => new CraftingBlacksmithEntityId(value);
    }

    public interface ICraftingBlacksmithNetworkAdapter
    {
        void SendRequest(CraftingBlacksmithRequest request);
        void SendResult(CraftingBlacksmithResult result);
    }

    public interface ICraftingBlacksmithPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
