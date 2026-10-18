using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.LocalizationLiveOps
{
    [CreateAssetMenu(fileName = "LocalizationLiveOpsConfig", menuName = "Game Systems/Localization & Live-Ops System/Config")]
    public sealed class LocalizationLiveOpsConfig : ScriptableObject
    {
        public string systemId = "SYS-40";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class LocalizationLiveOpsRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class LocalizationLiveOpsResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface ILocalizationLiveOpsAuthority
    {
        LocalizationLiveOpsResult ValidateAndExecute(LocalizationLiveOpsRequest request);
    }

    public sealed class LocalizationLiveOpsRuntime : MonoBehaviour
    {
        [SerializeField] private LocalizationLiveOpsConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public LocalizationLiveOpsConfig Config => config;

        public void Initialize(LocalizationLiveOpsConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(LocalizationLiveOpsRequest request, out LocalizationLiveOpsResult result)
        {
            result = new LocalizationLiveOpsResult { requestId = request != null ? request.requestId : string.Empty };
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
