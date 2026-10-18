using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.AdvancedLiquidWater
{
    [CreateAssetMenu(fileName = "AdvancedLiquidWaterConfig", menuName = "Game Systems/Advanced Liquid / Water System/Config")]
    public sealed class AdvancedLiquidWaterConfig : ScriptableObject
    {
        public string systemId = "SYS-06";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class AdvancedLiquidWaterRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class AdvancedLiquidWaterResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IAdvancedLiquidWaterAuthority
    {
        AdvancedLiquidWaterResult ValidateAndExecute(AdvancedLiquidWaterRequest request);
    }

    public sealed class AdvancedLiquidWaterRuntime : MonoBehaviour
    {
        [SerializeField] private AdvancedLiquidWaterConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public AdvancedLiquidWaterConfig Config => config;

        public void Initialize(AdvancedLiquidWaterConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(AdvancedLiquidWaterRequest request, out AdvancedLiquidWaterResult result)
        {
            result = new AdvancedLiquidWaterResult { requestId = request != null ? request.requestId : string.Empty };
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
