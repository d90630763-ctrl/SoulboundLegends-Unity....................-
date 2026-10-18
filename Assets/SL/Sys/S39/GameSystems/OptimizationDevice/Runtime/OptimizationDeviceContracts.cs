using System;

namespace GameSystems.OptimizationDevice
{
    public readonly struct OptimizationDeviceEntityId : IEquatable<OptimizationDeviceEntityId>
    {
        public readonly string Value;
        public OptimizationDeviceEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(OptimizationDeviceEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is OptimizationDeviceEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator OptimizationDeviceEntityId(string value) => new OptimizationDeviceEntityId(value);
    }

    public interface IOptimizationDeviceNetworkAdapter
    {
        void SendRequest(OptimizationDeviceRequest request);
        void SendResult(OptimizationDeviceResult result);
    }

    public interface IOptimizationDevicePersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
