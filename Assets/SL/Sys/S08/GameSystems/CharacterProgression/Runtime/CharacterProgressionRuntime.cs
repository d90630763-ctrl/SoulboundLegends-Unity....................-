using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.CharacterProgression
{
    [CreateAssetMenu(fileName = "CharacterProgressionConfig", menuName = "Game Systems/Character Progression System/Config")]
    public sealed class CharacterProgressionConfig : ScriptableObject
    {
        public string systemId = "SYS-08";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class CharacterProgressionRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class CharacterProgressionResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface ICharacterProgressionAuthority
    {
        CharacterProgressionResult ValidateAndExecute(CharacterProgressionRequest request);
    }

    public sealed class CharacterProgressionRuntime : MonoBehaviour
    {
        [SerializeField] private CharacterProgressionConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public CharacterProgressionConfig Config => config;

        public void Initialize(CharacterProgressionConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(CharacterProgressionRequest request, out CharacterProgressionResult result)
        {
            result = new CharacterProgressionResult { requestId = request != null ? request.requestId : string.Empty };
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
