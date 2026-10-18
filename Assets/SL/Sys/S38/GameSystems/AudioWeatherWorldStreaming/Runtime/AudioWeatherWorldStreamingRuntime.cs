using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.AudioWeatherWorldStreaming
{
    [CreateAssetMenu(fileName = "AudioWeatherWorldStreamingConfig", menuName = "Game Systems/Audio / Weather / World Streaming System/Config")]
    public sealed class AudioWeatherWorldStreamingConfig : ScriptableObject
    {
        public string systemId = "SYS-38";
        public bool enabledInBuild = true;
        [Min(0.001f)] public float serverRequestTimeout = 5f;
        [Min(1)] public int maxRequestsPerSecond = 20;
        [TextArea(2,6)] public string designNotes;
    }

    [Serializable]
    public sealed class AudioWeatherWorldStreamingRequest
    {
        public string requestId;
        public string entityId;
        public string action;
        public long clientSequence;
        public string payloadJson;
    }

    [Serializable]
    public sealed class AudioWeatherWorldStreamingResult
    {
        public bool accepted;
        public string requestId;
        public string reason;
        public long authoritativeSequence;
    }

    public interface IAudioWeatherWorldStreamingAuthority
    {
        AudioWeatherWorldStreamingResult ValidateAndExecute(AudioWeatherWorldStreamingRequest request);
    }

    public sealed class AudioWeatherWorldStreamingRuntime : MonoBehaviour
    {
        [SerializeField] private AudioWeatherWorldStreamingConfig config;
        private readonly HashSet<string> processedRequests = new HashSet<string>();

        public AudioWeatherWorldStreamingConfig Config => config;

        public void Initialize(AudioWeatherWorldStreamingConfig value)
        {
            config = value;
            if (config == null) throw new ArgumentNullException(nameof(value));
        }

        public bool TryAcceptRequest(AudioWeatherWorldStreamingRequest request, out AudioWeatherWorldStreamingResult result)
        {
            result = new AudioWeatherWorldStreamingResult { requestId = request != null ? request.requestId : string.Empty };
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
