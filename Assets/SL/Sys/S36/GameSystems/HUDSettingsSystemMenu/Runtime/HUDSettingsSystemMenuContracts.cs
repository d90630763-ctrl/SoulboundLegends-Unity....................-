using System;

namespace GameSystems.HUDSettingsSystemMenu
{
    public readonly struct HUDSettingsSystemMenuEntityId : IEquatable<HUDSettingsSystemMenuEntityId>
    {
        public readonly string Value;
        public HUDSettingsSystemMenuEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(HUDSettingsSystemMenuEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is HUDSettingsSystemMenuEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator HUDSettingsSystemMenuEntityId(string value) => new HUDSettingsSystemMenuEntityId(value);
    }

    public interface IHUDSettingsSystemMenuNetworkAdapter
    {
        void SendRequest(HUDSettingsSystemMenuRequest request);
        void SendResult(HUDSettingsSystemMenuResult result);
    }

    public interface IHUDSettingsSystemMenuPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
