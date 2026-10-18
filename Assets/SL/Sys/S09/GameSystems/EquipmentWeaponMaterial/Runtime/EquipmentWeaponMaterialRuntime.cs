using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.EquipmentWeaponMaterial
{
    [CreateAssetMenu(fileName = "EquipmentWeaponMaterialConfig", menuName = "Game Systems/Equipment, Weapon & Material System/Config")]
    public sealed class EquipmentWeaponMaterialConfig : ScriptableObject
    {
        public string systemId = "SYS-09";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class EquipmentWeaponMaterialRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class EquipmentWeaponMaterialResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IEquipmentWeaponMaterialAuthority
    {
        EquipmentWeaponMaterialResult ValidateAndExecute(EquipmentWeaponMaterialRequest request);
    }

    public sealed class EquipmentWeaponMaterialRuntime : MonoBehaviour
    {
        [SerializeField] private EquipmentWeaponMaterialConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public EquipmentWeaponMaterialConfig Config => config;

        public void Initialize(EquipmentWeaponMaterialConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(EquipmentWeaponMaterialRequest request, out EquipmentWeaponMaterialResult result)
        {
            result = new EquipmentWeaponMaterialResult { requestId = request != null ? request.requestId : string.Empty };
            if (request == null) { result.reason = "null_request"; return false; }
            if (string.IsNullOrWhiteSpace(request.requestId)) { result.reason = "missing_request_id"; return false; }
            if (processedRequests.Contains(request.requestId)) { result.reason = "duplicate_request"; return false; }
            processedRequests.Add(request.requestId);
            result.accepted = true;
            result.reason = "accepted";
            return true;
        }
    }
}
