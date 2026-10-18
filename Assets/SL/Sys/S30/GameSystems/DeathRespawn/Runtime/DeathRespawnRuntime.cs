using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.DeathRespawn
{
    [CreateAssetMenu(fileName = "DeathRespawnConfig", menuName = "Game Systems/Death & Respawn System/Config")]
    public sealed class DeathRespawnConfig : ScriptableObject
    {
        public string systemId = "SYS-30";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class DeathRespawnRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class DeathRespawnResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IDeathRespawnAuthority
    {
        DeathRespawnResult ValidateAndExecute(DeathRespawnRequest request);
    }

    public sealed class DeathRespawnRuntime : MonoBehaviour
    {
        [SerializeField] private DeathRespawnConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public DeathRespawnConfig Config => config;

        public void Initialize(DeathRespawnConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(DeathRespawnRequest request, out DeathRespawnResult result)
        {
            result = new DeathRespawnResult { requestId = request != null ? request.requestId : string.Empty };
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
