using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.AccountSession
{
    [CreateAssetMenu(fileName = "AccountSessionConfig", menuName = "Game Systems/Account & Session System/Config")]
    public sealed class AccountSessionConfig : ScriptableObject
    {
        public string systemId = "SYS-22";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class AccountSessionRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class AccountSessionResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IAccountSessionAuthority
    {
        AccountSessionResult ValidateAndExecute(AccountSessionRequest request);
    }

    public sealed class AccountSessionRuntime : MonoBehaviour
    {
        [SerializeField] private AccountSessionConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public AccountSessionConfig Config => config;

        public void Initialize(AccountSessionConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(AccountSessionRequest request, out AccountSessionResult result)
        {
            result = new AccountSessionResult { requestId = request != null ? request.requestId : string.Empty };
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
