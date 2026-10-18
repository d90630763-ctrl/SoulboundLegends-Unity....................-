using System;

namespace GameSystems.LocalizationLiveOps
{
    public readonly struct LocalizationLiveOpsEntityId : IEquatable<LocalizationLiveOpsEntityId>
    {
        public readonly string Value;
        public LocalizationLiveOpsEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(LocalizationLiveOpsEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is LocalizationLiveOpsEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator LocalizationLiveOpsEntityId(string value) => new LocalizationLiveOpsEntityId(value);
    }

    public interface ILocalizationLiveOpsNetworkAdapter
    {
        void SendRequest(LocalizationLiveOpsRequest request);
        void SendResult(LocalizationLiveOpsResult result);
    }

    public interface ILocalizationLiveOpsPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
