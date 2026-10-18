using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.DungeonBossEventInstance
{
    [CreateAssetMenu(fileName = "DungeonBossEventInstanceConfig", menuName = "Game Systems/Dungeon / Boss / Event Instance System/Config")]
    public sealed class DungeonBossEventInstanceConfig : ScriptableObject
    {
        public string systemId = "SYS-18";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class DungeonBossEventInstanceRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class DungeonBossEventInstanceResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IDungeonBossEventInstanceAuthority
    {
        DungeonBossEventInstanceResult ValidateAndExecute(DungeonBossEventInstanceRequest request);
    }

    public sealed class DungeonBossEventInstanceRuntime : MonoBehaviour
    {
        [SerializeField] private DungeonBossEventInstanceConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public DungeonBossEventInstanceConfig Config => config;

        public void Initialize(DungeonBossEventInstanceConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(DungeonBossEventInstanceRequest request, out DungeonBossEventInstanceResult result)
        {
            result = new DungeonBossEventInstanceResult { requestId = request != null ? request.requestId : string.Empty };
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
