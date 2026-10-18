using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.MapNavigation
{
    [CreateAssetMenu(fileName = "MapNavigationConfig", menuName = "Game Systems/Map & Navigation System/Config")]
    public sealed class MapNavigationConfig : ScriptableObject
    {
        public string systemId = "SYS-37";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class MapNavigationRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class MapNavigationResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IMapNavigationAuthority
    {
        MapNavigationResult ValidateAndExecute(MapNavigationRequest request);
    }

    public sealed class MapNavigationRuntime : MonoBehaviour
    {
        [SerializeField] private MapNavigationConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public MapNavigationConfig Config => config;

        public void Initialize(MapNavigationConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(MapNavigationRequest request, out MapNavigationResult result)
        {
            result = new MapNavigationResult { requestId = request != null ? request.requestId : string.Empty };
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
