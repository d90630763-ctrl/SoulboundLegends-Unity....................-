using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.DynamicInnInteriorInstance
{
    [CreateAssetMenu(fileName = "DynamicInnInteriorInstanceConfig", menuName = "Game Systems/Dynamic Inn / Interior Instance System/Config")]
    public sealed class DynamicInnInteriorInstanceConfig : ScriptableObject
    {
        public string systemId = "SYS-21";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class DynamicInnInteriorInstanceRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class DynamicInnInteriorInstanceResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IDynamicInnInteriorInstanceAuthority
    {
        DynamicInnInteriorInstanceResult ValidateAndExecute(DynamicInnInteriorInstanceRequest request);
    }

    public sealed class DynamicInnInteriorInstanceRuntime : MonoBehaviour
    {
        [SerializeField] private DynamicInnInteriorInstanceConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public DynamicInnInteriorInstanceConfig Config => config;

        public void Initialize(DynamicInnInteriorInstanceConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(DynamicInnInteriorInstanceRequest request, out DynamicInnInteriorInstanceResult result)
        {
            result = new DynamicInnInteriorInstanceResult { requestId = request != null ? request.requestId : string.Empty };
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
