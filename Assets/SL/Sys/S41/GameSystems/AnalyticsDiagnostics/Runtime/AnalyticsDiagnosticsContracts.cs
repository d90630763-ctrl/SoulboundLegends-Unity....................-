using System;

namespace GameSystems.AnalyticsDiagnostics
{
    public readonly struct AnalyticsDiagnosticsEntityId : IEquatable<AnalyticsDiagnosticsEntityId>
    {
        public readonly string Value;
        public AnalyticsDiagnosticsEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AnalyticsDiagnosticsEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AnalyticsDiagnosticsEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AnalyticsDiagnosticsEntityId(string value) => new AnalyticsDiagnosticsEntityId(value);
    }

    public interface IAnalyticsDiagnosticsNetworkAdapter
    {
        void SendRequest(AnalyticsDiagnosticsRequest request);
        void SendResult(AnalyticsDiagnosticsResult result);
    }

    public interface IAnalyticsDiagnosticsPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
