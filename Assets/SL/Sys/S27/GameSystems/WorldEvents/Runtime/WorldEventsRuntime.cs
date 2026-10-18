using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.WorldEvents
{
    [CreateAssetMenu(fileName = "WorldEventsConfig", menuName = "Game Systems/World Events System/Config")]
    public sealed class WorldEventsConfig : ScriptableObject
    {
        public string systemId = "SYS-27";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class WorldEventsRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class WorldEventsResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IWorldEventsAuthority
    {
        WorldEventsResult ValidateAndExecute(WorldEventsRequest request);
    }

    public sealed class WorldEventsRuntime : MonoBehaviour
    {
        [SerializeField] private WorldEventsConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public WorldEventsConfig Config => config;

        public void Initialize(WorldEventsConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(WorldEventsRequest request, out WorldEventsResult result)
        {
            result = new WorldEventsResult { requestId = request != null ? request.requestId : string.Empty };
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
