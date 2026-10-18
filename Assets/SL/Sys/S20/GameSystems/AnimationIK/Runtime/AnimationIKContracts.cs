using System;

namespace GameSystems.AnimationIK
{
    public readonly struct AnimationIKEntityId : IEquatable<AnimationIKEntityId>
    {
        public readonly string Value;
        public AnimationIKEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AnimationIKEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AnimationIKEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AnimationIKEntityId(string value) => new AnimationIKEntityId(value);
    }

    public interface IAnimationIKNetworkAdapter
    {
        void SendRequest(AnimationIKRequest request);
        void SendResult(AnimationIKResult result);
    }

    public interface IAnimationIKPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
