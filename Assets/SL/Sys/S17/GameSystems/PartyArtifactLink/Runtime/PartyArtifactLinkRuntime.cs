using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.PartyArtifactLink
{
    [CreateAssetMenu(fileName = "PartyArtifactLinkConfig", menuName = "Game Systems/Party / Artifact Link System/Config")]
    public sealed class PartyArtifactLinkConfig : ScriptableObject
    {
        public string systemId = "SYS-17";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class PartyArtifactLinkRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class PartyArtifactLinkResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IPartyArtifactLinkAuthority
    {
        PartyArtifactLinkResult ValidateAndExecute(PartyArtifactLinkRequest request);
    }

    public sealed class PartyArtifactLinkRuntime : MonoBehaviour
    {
        [SerializeField] private PartyArtifactLinkConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public PartyArtifactLinkConfig Config => config;

        public void Initialize(PartyArtifactLinkConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(PartyArtifactLinkRequest request, out PartyArtifactLinkResult result)
        {
            result = new PartyArtifactLinkResult { requestId = request != null ? request.requestId : string.Empty };
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
