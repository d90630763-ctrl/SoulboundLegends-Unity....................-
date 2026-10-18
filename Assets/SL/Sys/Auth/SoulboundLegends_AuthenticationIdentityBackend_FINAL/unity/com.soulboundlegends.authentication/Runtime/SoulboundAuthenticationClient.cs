using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundLegends.Authentication
{
    [Serializable] public sealed class AuthResponse
    {
        public string accountId;
        public string sessionId;
        public string accessToken;
        public string refreshToken;
        public string accessTokenExpiresAt;
        public string sessionExpiresAt;
        public bool characterExists;
        public string playerId;
        public string characterId;
    }

    [Serializable] public sealed class PasswordLoginRequest
    {
        public string Email;
        public string Password;
        public string DeviceId;
    }

    [Serializable] public sealed class GoogleLoginRequest
    {
        public string IdToken;
        public string Nonce;
        public string DeviceId;
    }

    [Serializable] public sealed class RefreshRequest
    {
        public string RefreshToken;
        public string DeviceId;
    }

    public sealed class SoulboundAuthenticationClient
    {
        private readonly HttpClient http;
        private readonly string baseUrl;
        private readonly JsonSerializerOptions json = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string AccountId { get; private set; }
        public string SessionId { get; private set; }
        public bool CharacterExists { get; private set; }
        public string PlayerId { get; private set; }
        public string CharacterId { get; private set; }

        public SoulboundAuthenticationClient(string authenticationBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(authenticationBaseUrl)) throw new ArgumentException("Authentication URL is required.");
            baseUrl = authenticationBaseUrl.TrimEnd('/');
            http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        }

        public async Task<AuthResponse> LoginWithPasswordAsync(string email, string password, string deviceId, CancellationToken ct)
        {
            return await PostAsync<PasswordLoginRequest, AuthResponse>("/api/v1/auth/login/password",
                new PasswordLoginRequest { Email = email, Password = password, DeviceId = deviceId }, ct, false);
        }

        public async Task<AuthResponse> LoginWithGoogleAsync(string idToken, string nonce, string deviceId, CancellationToken ct)
        {
            return await PostAsync<GoogleLoginRequest, AuthResponse>("/api/v1/auth/login/google",
                new GoogleLoginRequest { IdToken = idToken, Nonce = nonce, DeviceId = deviceId }, ct, false);
        }

        public async Task<AuthResponse> RefreshAsync(string deviceId, CancellationToken ct)
        {
            return await PostAsync<RefreshRequest, AuthResponse>("/api/v1/auth/refresh",
                new RefreshRequest { RefreshToken = RefreshToken, DeviceId = deviceId }, ct, false);
        }

        public async Task LogoutAsync(bool allDevices, CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/api/v1/auth/logout");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            req.Content = new StringContent(JsonSerializer.Serialize(new { allDevices }, json), Encoding.UTF8, "application/json");
            using var response = await http.SendAsync(req, ct);
            if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await response.Content.ReadAsStringAsync(ct));
            Clear();
        }

        public void Apply(AuthResponse r)
        {
            AccessToken = r.accessToken;
            RefreshToken = r.refreshToken;
            AccountId = r.accountId;
            SessionId = r.sessionId;
            CharacterExists = r.characterExists;
            PlayerId = r.playerId;
            CharacterId = r.characterId;
        }

        public void Clear()
        {
            AccessToken = null;
            RefreshToken = null;
            AccountId = null;
            SessionId = null;
            CharacterExists = false;
            PlayerId = null;
            CharacterId = null;
        }

        private async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct, bool authenticated)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, baseUrl + path);
            if (authenticated) req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            req.Content = new StringContent(JsonSerializer.Serialize(body, json), Encoding.UTF8, "application/json");
            using var response = await http.SendAsync(req, ct);
            var text = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode) throw new InvalidOperationException(text);
            var result = JsonSerializer.Deserialize<TResponse>(text, json);
            if (result == null) throw new InvalidOperationException("Authentication server returned an invalid response.");
            if (result is AuthResponse auth) Apply(auth);
            return result;
        }
    }
}
