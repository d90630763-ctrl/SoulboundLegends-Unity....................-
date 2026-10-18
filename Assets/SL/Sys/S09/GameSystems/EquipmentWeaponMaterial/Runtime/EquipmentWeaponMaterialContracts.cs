using System;

namespace GameSystems.EquipmentWeaponMaterial
{
    public readonly struct EquipmentWeaponMaterialEntityId : IEquatable<EquipmentWeaponMaterialEntityId>
    {
        public readonly string Value;
        public EquipmentWeaponMaterialEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(EquipmentWeaponMaterialEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EquipmentWeaponMaterialEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator EquipmentWeaponMaterialEntityId(string value) => new EquipmentWeaponMaterialEntityId(value);
    }

    public interface IEquipmentWeaponMaterialNetworkAdapter
    {
        void SendRequest(EquipmentWeaponMaterialRequest request);
        void SendResult(EquipmentWeaponMaterialResult result);
    }

    public interface IEquipmentWeaponMaterialPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
