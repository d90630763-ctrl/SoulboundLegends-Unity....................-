using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.SecurityAntiCheat
{
    [CreateAssetMenu(fileName = "SecurityAntiCheatConfig", menuName = "Game Systems/Security / Anti-Cheat System/Config")]
    public sealed class SecurityAntiCheatConfig : ScriptableObject
    {
        public string systemId = "SYS-32";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class SecurityAntiCheatRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class SecurityAntiCheatResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface ISecurityAntiCheatAuthority
    {
        SecurityAntiCheatResult ValidateAndExecute(SecurityAntiCheatRequest request);
    }

    public sealed class SecurityAntiCheatRuntime : MonoBehaviour
    {
        [SerializeField] private SecurityAntiCheatConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public SecurityAntiCheatConfig Config => config;

        public void Initialize(SecurityAntiCheatConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(SecurityAntiCheatRequest request, out SecurityAntiCheatResult result)
        {
            result = new SecurityAntiCheatResult { requestId = request != null ? request.requestId : string.Empty };
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
