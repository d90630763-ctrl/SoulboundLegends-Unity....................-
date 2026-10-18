using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SoulboundLegends.Authority
{
    [DefaultExecutionOrder(-10000)]
    public sealed class MasterAuthorityClient : MonoBehaviour
    {
        public static MasterAuthorityClient Instance { get; private set; }

        [Header("Server")]
        [SerializeField] private string apiBaseUrl = "https://api.soulboundlegends.com";
        [SerializeField] private int timeoutSeconds = 15;
        [SerializeField] private int maxRequestsPerSecondPerSystem = 8;

        [Header("Security")]
        [SerializeField] private bool requireHttpsOutsideEditor = true;
        [SerializeField] private bool verboseLogging = false;
        [SerializeField] private bool blockUnknownSystems = true;

        [Header("Universal Authority")]
        [SerializeField] private AuthoritySystemRegistry systemRegistry;

        private string accessToken;
        private string playerId;
        private readonly Dictionary<string, Queue<float>> requestTimes = new(StringComparer.OrdinalIgnoreCase);

        public bool IsAuthenticated => !string.IsNullOrEmpty(accessToken);
        public string PlayerId => playerId;
        public string ApiBaseUrl => apiBaseUrl;
        public AuthoritySystemRegistry SystemRegistry => systemRegistry;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (systemRegistry == null) systemRegistry = GetComponent<AuthoritySystemRegistry>();
            if (systemRegistry == null) systemRegistry = AuthoritySystemRegistry.FindInScene();
            if (systemRegistry != null) systemRegistry.Initialize(this);

#if !UNITY_EDITOR
            if (requireHttpsOutsideEditor && !apiBaseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                Debug.LogError("Soulbound Authority: production API must use HTTPS.");
#endif
        }

        public void Configure(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("API URL cannot be empty.");
            apiBaseUrl = url.TrimEnd('/');
        }

        public void SetSession(string token, string authenticatedPlayerId)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Access token cannot be empty.");
            accessToken = token;
            playerId = authenticatedPlayerId;
        }

        public void ClearSession() { accessToken = null; playerId = null; }

        public IEnumerator Authorize(
            string systemId,
            string capability,
            string action,
            Dictionary<string, object> payload,
            Action<AuthorityResponse> completed)
        {
            if (!IsAuthenticated)
            {
                completed?.Invoke(new AuthorityResponse { code = AuthorityResultCode.Unauthorized, systemId = systemId, action = action, message = "Player is not authenticated." });
                yield break;
            }

            if (blockUnknownSystems && (systemRegistry == null || !systemRegistry.IsSystemConnected(systemId)))
            {
                completed?.Invoke(new AuthorityResponse { code = AuthorityResultCode.Forbidden, systemId = systemId, action = action, message = "System is not connected to Master Authority." });
                yield break;
            }

            if (!CheckClientRate(systemId))
            {
                completed?.Invoke(new AuthorityResponse { code = AuthorityResultCode.RateLimited, systemId = systemId, action = action, message = "Local safety rate limit reached." });
                yield break;
            }

            var requestId = Guid.NewGuid().ToString("N");
            var body = JsonUtility.ToJson(new SerializableActionRequest
            {
                requestId = requestId,
                systemId = systemId,
                capability = capability ?? string.Empty,
                action = action,
                clientTick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                payloadJson = MiniJson.Serialize(payload ?? new Dictionary<string, object>())
            });

            using var request = new UnityWebRequest(apiBaseUrl + "/api/v2/authority/authorize", UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = timeoutSeconds;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            request.SetRequestHeader("X-Request-Id", requestId);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (verboseLogging) Debug.LogWarning($"Master Authority request failed: {request.responseCode} {request.error}");
                completed?.Invoke(new AuthorityResponse { code = request.responseCode == 429 ? AuthorityResultCode.RateLimited : AuthorityResultCode.ServerError, requestId = requestId, systemId = systemId, action = action, message = request.error });
                yield break;
            }

            var response = JsonUtility.FromJson<AuthorityResponse>(request.downloadHandler.text);
            completed?.Invoke(response);
        }

        public IEnumerator GetSnapshot(Action<PlayerSnapshot> completed)
        {
            if (!IsAuthenticated) { completed?.Invoke(null); yield break; }
            using var request = UnityWebRequest.Get(apiBaseUrl + "/api/v1/player/snapshot");
            request.timeout = timeoutSeconds;
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success) { completed?.Invoke(null); yield break; }
            var snapshot = JsonUtility.FromJson<PlayerSnapshot>(request.downloadHandler.text);
            if (systemRegistry != null)
                foreach (var system in systemRegistry.Systems) system.OnAuthoritySnapshot(snapshot);
            completed?.Invoke(snapshot);
        }

        private bool CheckClientRate(string systemId)
        {
            var now = Time.unscaledTime;
            if (!requestTimes.TryGetValue(systemId, out var q)) requestTimes[systemId] = q = new Queue<float>();
            while (q.Count > 0 && now - q.Peek() >= 1f) q.Dequeue();
            if (q.Count >= Mathf.Max(1, maxRequestsPerSecondPerSystem)) return false;
            q.Enqueue(now); return true;
        }

        [Serializable] private sealed class SerializableActionRequest
        {
            public string requestId, systemId, capability, action;
            public long clientTick;
            public string payloadJson;
        }
    }
}