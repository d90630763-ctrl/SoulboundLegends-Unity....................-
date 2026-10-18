using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.OptimizationDevice
{
    [CreateAssetMenu(fileName = "OptimizationDeviceConfig", menuName = "Game Systems/Optimization & Device System/Config")]
    public sealed class OptimizationDeviceConfig : ScriptableObject
    {
        public string systemId = "SYS-39";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class OptimizationDeviceRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class OptimizationDeviceResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IOptimizationDeviceAuthority
    {
        OptimizationDeviceResult ValidateAndExecute(OptimizationDeviceRequest request);
    }

    public sealed class OptimizationDeviceRuntime : MonoBehaviour
    {
        [SerializeField] private OptimizationDeviceConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public OptimizationDeviceConfig Config => config;

        public void Initialize(OptimizationDeviceConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(OptimizationDeviceRequest request, out OptimizationDeviceResult result)
        {
            result = new OptimizationDeviceResult { requestId = request != null ? request.requestId : string.Empty };
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
