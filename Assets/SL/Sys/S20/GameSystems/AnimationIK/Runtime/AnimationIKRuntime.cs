using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.AnimationIK
{
    [CreateAssetMenu(fileName = "AnimationIKConfig", menuName = "Game Systems/Animation & IK System/Config")]
    public sealed class AnimationIKConfig : ScriptableObject
    {
        public string systemId = "SYS-20";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class AnimationIKRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class AnimationIKResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IAnimationIKAuthority
    {
        AnimationIKResult ValidateAndExecute(AnimationIKRequest request);
    }

    public sealed class AnimationIKRuntime : MonoBehaviour
    {
        [SerializeField] private AnimationIKConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public AnimationIKConfig Config => config;

        public void Initialize(AnimationIKConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(AnimationIKRequest request, out AnimationIKResult result)
        {
            result = new AnimationIKResult { requestId = request != null ? request.requestId : string.Empty };
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
