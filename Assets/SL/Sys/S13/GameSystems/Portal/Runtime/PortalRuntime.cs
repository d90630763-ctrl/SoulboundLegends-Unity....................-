using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.Portal
{
    [CreateAssetMenu(fileName = "PortalConfig", menuName = "Game Systems/Portal System/Config")]
    public sealed class PortalConfig : ScriptableObject
    {
        public string systemId = "SYS-13";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class PortalRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class PortalResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IPortalAuthority
    {
        PortalResult ValidateAndExecute(PortalRequest request);
    }

    public sealed class PortalRuntime : MonoBehaviour
    {
        [SerializeField] private PortalConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public PortalConfig Config => config;

        public void Initialize(PortalConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(PortalRequest request, out PortalResult result)
        {
            result = new PortalResult { requestId = request != null ? request.requestId : string.Empty };
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
