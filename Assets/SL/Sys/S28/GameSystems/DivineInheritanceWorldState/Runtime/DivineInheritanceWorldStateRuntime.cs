using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.DivineInheritanceWorldState
{
    [CreateAssetMenu(fileName = "DivineInheritanceWorldStateConfig", menuName = "Game Systems/Divine Inheritance / World-State System/Config")]
    public sealed class DivineInheritanceWorldStateConfig : ScriptableObject
    {
        public string systemId = "SYS-28";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class DivineInheritanceWorldStateRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class DivineInheritanceWorldStateResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IDivineInheritanceWorldStateAuthority
    {
        DivineInheritanceWorldStateResult ValidateAndExecute(DivineInheritanceWorldStateRequest request);
    }

    public sealed class DivineInheritanceWorldStateRuntime : MonoBehaviour
    {
        [SerializeField] private DivineInheritanceWorldStateConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public DivineInheritanceWorldStateConfig Config => config;

        public void Initialize(DivineInheritanceWorldStateConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(DivineInheritanceWorldStateRequest request, out DivineInheritanceWorldStateResult result)
        {
            result = new DivineInheritanceWorldStateResult { requestId = request != null ? request.requestId : string.Empty };
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
