using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.NotificationsTutorial
{
    [CreateAssetMenu(fileName = "NotificationsTutorialConfig", menuName = "Game Systems/Notifications / Tutorial System/Config")]
    public sealed class NotificationsTutorialConfig : ScriptableObject
    {
        public string systemId = "SYS-34";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class NotificationsTutorialRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class NotificationsTutorialResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface INotificationsTutorialAuthority
    {
        NotificationsTutorialResult ValidateAndExecute(NotificationsTutorialRequest request);
    }

    public sealed class NotificationsTutorialRuntime : MonoBehaviour
    {
        [SerializeField] private NotificationsTutorialConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public NotificationsTutorialConfig Config => config;

        public void Initialize(NotificationsTutorialConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(NotificationsTutorialRequest request, out NotificationsTutorialResult result)
        {
            result = new NotificationsTutorialResult { requestId = request != null ? request.requestId : string.Empty };
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
