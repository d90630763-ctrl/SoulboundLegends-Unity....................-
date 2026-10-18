using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.SocialFriendsChat
{
    [CreateAssetMenu(fileName = "SocialFriendsChatConfig", menuName = "Game Systems/Social / Friends / Chat System/Config")]
    public sealed class SocialFriendsChatConfig : ScriptableObject
    {
        public string systemId = "SYS-25";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class SocialFriendsChatRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class SocialFriendsChatResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface ISocialFriendsChatAuthority
    {
        SocialFriendsChatResult ValidateAndExecute(SocialFriendsChatRequest request);
    }

    public sealed class SocialFriendsChatRuntime : MonoBehaviour
    {
        [SerializeField] private SocialFriendsChatConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public SocialFriendsChatConfig Config => config;

        public void Initialize(SocialFriendsChatConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(SocialFriendsChatRequest request, out SocialFriendsChatResult result)
        {
            result = new SocialFriendsChatResult { requestId = request != null ? request.requestId : string.Empty };
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
