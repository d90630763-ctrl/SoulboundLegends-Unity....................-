using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.GeneralMonster
{
    [CreateAssetMenu(fileName = "GeneralMonsterConfig", menuName = "Game Systems/General Monster System/Config")]
    public sealed class GeneralMonsterConfig : ScriptableObject
    {
        public string systemId = "SYS-02";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class GeneralMonsterRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class GeneralMonsterResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IGeneralMonsterAuthority
    {
        GeneralMonsterResult ValidateAndExecute(GeneralMonsterRequest request);
    }

    public sealed class GeneralMonsterRuntime : MonoBehaviour
    {
        [SerializeField] private GeneralMonsterConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public GeneralMonsterConfig Config => config;

        public void Initialize(GeneralMonsterConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(GeneralMonsterRequest request, out GeneralMonsterResult result)
        {
            result = new GeneralMonsterResult { requestId = request != null ? request.requestId : string.Empty };
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
