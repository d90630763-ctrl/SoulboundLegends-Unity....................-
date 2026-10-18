using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.MultiplayerAOI
{
    [CreateAssetMenu(fileName = "MultiplayerAOIConfig", menuName = "Game Systems/Multiplayer AOI System/Config")]
    public sealed class MultiplayerAOIConfig : ScriptableObject
    {
        public string systemId = "SYS-14";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class MultiplayerAOIRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class MultiplayerAOIResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IMultiplayerAOIAuthority
    {
        MultiplayerAOIResult ValidateAndExecute(MultiplayerAOIRequest request);
    }

    public sealed class MultiplayerAOIRuntime : MonoBehaviour
    {
        [SerializeField] private MultiplayerAOIConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public MultiplayerAOIConfig Config => config;

        public void Initialize(MultiplayerAOIConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(MultiplayerAOIRequest request, out MultiplayerAOIResult result)
        {
            result = new MultiplayerAOIResult { requestId = request != null ? request.requestId : string.Empty };
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
