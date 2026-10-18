using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.GeneralMobileControls
{
    [CreateAssetMenu(fileName = "GeneralMobileControlsConfig", menuName = "Game Systems/General Mobile Controls System/Config")]
    public sealed class GeneralMobileControlsConfig : ScriptableObject
    {
        public string systemId = "SYS-35";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class GeneralMobileControlsRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class GeneralMobileControlsResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IGeneralMobileControlsAuthority
    {
        GeneralMobileControlsResult ValidateAndExecute(GeneralMobileControlsRequest request);
    }

    public sealed class GeneralMobileControlsRuntime : MonoBehaviour
    {
        [SerializeField] private GeneralMobileControlsConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public GeneralMobileControlsConfig Config => config;

        public void Initialize(GeneralMobileControlsConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(GeneralMobileControlsRequest request, out GeneralMobileControlsResult result)
        {
            result = new GeneralMobileControlsResult { requestId = request != null ? request.requestId : string.Empty };
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
