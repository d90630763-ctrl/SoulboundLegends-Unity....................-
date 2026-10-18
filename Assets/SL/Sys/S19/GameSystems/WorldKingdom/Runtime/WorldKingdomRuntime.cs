using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.WorldKingdom
{
    [CreateAssetMenu(fileName = "WorldKingdomConfig", menuName = "Game Systems/World / Kingdom System/Config")]
    public sealed class WorldKingdomConfig : ScriptableObject
    {
        public string systemId = "SYS-19";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class WorldKingdomRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class WorldKingdomResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IWorldKingdomAuthority
    {
        WorldKingdomResult ValidateAndExecute(WorldKingdomRequest request);
    }

    public sealed class WorldKingdomRuntime : MonoBehaviour
    {
        [SerializeField] private WorldKingdomConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public WorldKingdomConfig Config => config;

        public void Initialize(WorldKingdomConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(WorldKingdomRequest request, out WorldKingdomResult result)
        {
            result = new WorldKingdomResult { requestId = request != null ? request.requestId : string.Empty };
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
