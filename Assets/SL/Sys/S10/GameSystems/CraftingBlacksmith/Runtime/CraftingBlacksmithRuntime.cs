using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.CraftingBlacksmith
{
    [CreateAssetMenu(fileName = "CraftingBlacksmithConfig", menuName = "Game Systems/Crafting & Blacksmith System/Config")]
    public sealed class CraftingBlacksmithConfig : ScriptableObject
    {
        public string systemId = "SYS-10";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class CraftingBlacksmithRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class CraftingBlacksmithResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface ICraftingBlacksmithAuthority
    {
        CraftingBlacksmithResult ValidateAndExecute(CraftingBlacksmithRequest request);
    }

    public sealed class CraftingBlacksmithRuntime : MonoBehaviour
    {
        [SerializeField] private CraftingBlacksmithConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public CraftingBlacksmithConfig Config => config;

        public void Initialize(CraftingBlacksmithConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(CraftingBlacksmithRequest request, out CraftingBlacksmithResult result)
        {
            result = new CraftingBlacksmithResult { requestId = request != null ? request.requestId : string.Empty };
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
