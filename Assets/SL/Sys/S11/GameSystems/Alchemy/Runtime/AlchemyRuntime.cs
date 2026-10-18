using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.Alchemy
{
    [CreateAssetMenu(fileName = "AlchemyConfig", menuName = "Game Systems/Alchemy System/Config")]
    public sealed class AlchemyConfig : ScriptableObject
    {
        public string systemId = "SYS-11";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class AlchemyRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class AlchemyResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IAlchemyAuthority
    {
        AlchemyResult ValidateAndExecute(AlchemyRequest request);
    }

    public sealed class AlchemyRuntime : MonoBehaviour
    {
        [SerializeField] private AlchemyConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public AlchemyConfig Config => config;

        public void Initialize(AlchemyConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(AlchemyRequest request, out AlchemyResult result)
        {
            result = new AlchemyResult { requestId = request != null ? request.requestId : string.Empty };
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
