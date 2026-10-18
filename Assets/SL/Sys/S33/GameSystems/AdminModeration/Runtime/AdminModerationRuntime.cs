using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.AdminModeration
{
    [CreateAssetMenu(fileName = "AdminModerationConfig", menuName = "Game Systems/Admin / Moderation System/Config")]
    public sealed class AdminModerationConfig : ScriptableObject
    {
        public string systemId = "SYS-33";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class AdminModerationRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class AdminModerationResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IAdminModerationAuthority
    {
        AdminModerationResult ValidateAndExecute(AdminModerationRequest request);
    }

    public sealed class AdminModerationRuntime : MonoBehaviour
    {
        [SerializeField] private AdminModerationConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public AdminModerationConfig Config => config;

        public void Initialize(AdminModerationConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(AdminModerationRequest request, out AdminModerationResult result)
        {
            result = new AdminModerationResult { requestId = request != null ? request.requestId : string.Empty };
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
