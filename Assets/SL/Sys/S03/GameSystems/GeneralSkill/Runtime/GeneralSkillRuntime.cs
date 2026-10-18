using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.GeneralSkill
{
    [CreateAssetMenu(fileName = "GeneralSkillConfig", menuName = "Game Systems/General Skill System/Config")]
    public sealed class GeneralSkillConfig : ScriptableObject
    {
        public string systemId = "SYS-03";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class GeneralSkillRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class GeneralSkillResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IGeneralSkillAuthority
    {
        GeneralSkillResult ValidateAndExecute(GeneralSkillRequest request);
    }

    public sealed class GeneralSkillRuntime : MonoBehaviour
    {
        [SerializeField] private GeneralSkillConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public GeneralSkillConfig Config => config;

        public void Initialize(GeneralSkillConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(GeneralSkillRequest request, out GeneralSkillResult result)
        {
            result = new GeneralSkillResult { requestId = request != null ? request.requestId : string.Empty };
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
