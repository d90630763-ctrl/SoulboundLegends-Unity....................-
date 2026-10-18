using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.MonsterHumanGeneration
{
    [CreateAssetMenu(fileName = "MonsterHumanGenerationConfig", menuName = "Game Systems/Monster & Human Generation System/Config")]
    public sealed class MonsterHumanGenerationConfig : ScriptableObject
    {
        public string systemId = "SYS-01";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class MonsterHumanGenerationRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class MonsterHumanGenerationResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IMonsterHumanGenerationAuthority
    {
        MonsterHumanGenerationResult ValidateAndExecute(MonsterHumanGenerationRequest request);
    }

    public sealed class MonsterHumanGenerationRuntime : MonoBehaviour
    {
        [SerializeField] private MonsterHumanGenerationConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public MonsterHumanGenerationConfig Config => config;

        public void Initialize(MonsterHumanGenerationConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(MonsterHumanGenerationRequest request, out MonsterHumanGenerationResult result)
        {
            result = new MonsterHumanGenerationResult { requestId = request != null ? request.requestId : string.Empty };
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
