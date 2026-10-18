using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.ServerAuthorizationSecurity
{
    [CreateAssetMenu(fileName = "ServerAuthorizationSecurityConfig", menuName = "Game Systems/Server Authorization / Security System/Config")]
    public sealed class ServerAuthorizationSecurityConfig : ScriptableObject
    {
        public string systemId = "SYS-15";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class ServerAuthorizationSecurityRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class ServerAuthorizationSecurityResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IServerAuthorizationSecurityAuthority
    {
        ServerAuthorizationSecurityResult ValidateAndExecute(ServerAuthorizationSecurityRequest request);
    }

    public sealed class ServerAuthorizationSecurityRuntime : MonoBehaviour
    {
        [SerializeField] private ServerAuthorizationSecurityConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public ServerAuthorizationSecurityConfig Config => config;

        public void Initialize(ServerAuthorizationSecurityConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(ServerAuthorizationSecurityRequest request, out ServerAuthorizationSecurityResult result)
        {
            result = new ServerAuthorizationSecurityResult { requestId = request != null ? request.requestId : string.Empty };
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
