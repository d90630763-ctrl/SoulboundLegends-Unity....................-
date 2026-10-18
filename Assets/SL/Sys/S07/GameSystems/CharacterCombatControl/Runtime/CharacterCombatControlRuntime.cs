using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.CharacterCombatControl
{
    [CreateAssetMenu(fileName = "CharacterCombatControlConfig", menuName = "Game Systems/Character & Combat Control System/Config")]
    public sealed class CharacterCombatControlConfig : ScriptableObject
    {
        public string systemId = "SYS-07";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class CharacterCombatControlRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class CharacterCombatControlResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface ICharacterCombatControlAuthority
    {
        CharacterCombatControlResult ValidateAndExecute(CharacterCombatControlRequest request);
    }

    public sealed class CharacterCombatControlRuntime : MonoBehaviour
    {
        [SerializeField] private CharacterCombatControlConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public CharacterCombatControlConfig Config => config;

        public void Initialize(CharacterCombatControlConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(CharacterCombatControlRequest request, out CharacterCombatControlResult result)
        {
            result = new CharacterCombatControlResult { requestId = request != null ? request.requestId : string.Empty };
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
