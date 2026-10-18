using System;

namespace GameSystems.GeneralMobileControls
{
    public readonly struct GeneralMobileControlsEntityId : IEquatable<GeneralMobileControlsEntityId>
    {
        public readonly string Value;
        public GeneralMobileControlsEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(GeneralMobileControlsEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is GeneralMobileControlsEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator GeneralMobileControlsEntityId(string value) => new GeneralMobileControlsEntityId(value);
    }

    public interface IGeneralMobileControlsNetworkAdapter
    {
        void SendRequest(GeneralMobileControlsRequest request);
        void SendResult(GeneralMobileControlsResult result);
    }

    public interface IGeneralMobileControlsPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
