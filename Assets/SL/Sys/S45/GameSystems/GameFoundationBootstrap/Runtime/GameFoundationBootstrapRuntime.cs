using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.GameFoundationBootstrap
{
    [CreateAssetMenu(fileName = "GameFoundationBootstrapConfig", menuName = "Game Systems/Game Foundation Bootstrap/Config")]
    public sealed class GameFoundationBootstrapConfig : ScriptableObject
    {
        public string systemId = "SYS-45";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class GameFoundationBootstrapRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class GameFoundationBootstrapResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IGameFoundationBootstrapAuthority
    {
        GameFoundationBootstrapResult ValidateAndExecute(GameFoundationBootstrapRequest request);
    }

    public sealed class GameFoundationBootstrapRuntime : MonoBehaviour
    {
        [SerializeField] private GameFoundationBootstrapConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public GameFoundationBootstrapConfig Config => config;

        public void Initialize(GameFoundationBootstrapConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(GameFoundationBootstrapRequest request, out GameFoundationBootstrapResult result)
        {
            result = new GameFoundationBootstrapResult { requestId = request != null ? request.requestId : string.Empty };
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
