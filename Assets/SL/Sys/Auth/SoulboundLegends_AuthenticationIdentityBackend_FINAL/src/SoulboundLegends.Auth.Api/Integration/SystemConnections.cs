using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SoulboundLegends.Auth.Api.Configuration;

namespace SoulboundLegends.Auth.Api.Integration;

public interface ISystemConnectionRegistry
{
    IReadOnlyDictionary<string, string> GetConnections();
}

public sealed class SystemConnectionRegistry(IOptions<IntegrationOptions> options) : ISystemConnectionRegistry
{
    private readonly IntegrationOptions _o = options.Value;
    public IReadOnlyDictionary<string, string> GetConnections() => new Dictionary<string, string>
    {
        ["Account & Session"] = "Internal authentication session state is authoritative here; external session orchestration can subscribe through the adapter contract.",
        ["Server Authority"] = _o.ServerAuthorityBaseUrl,
        ["Player Database Backend"] = _o.PlayerDatabaseBaseUrl,
        ["Security / Anti-Cheat"] = _o.AntiCheatBaseUrl,
        ["Network Transport"] = "Transport consumes the authenticated session/token contract.",
        ["Persistence"] = "PostgreSQL via AuthDbContext.",
        ["Lobby"] = "Consumes authentication response and identity state.",
        ["Character/Human Generation"] = "Invoked by Character Creation integration after CharacterExists=false.",
        ["Character Progression"] = "Consumes PlayerId/CharacterId after identity creation.",
        ["Master Registry"] = _o.MasterRegistryBaseUrl
    };
}

public interface IPlayerIdentityGateway
{
    Task<PlayerIdentityState> GetIdentityAsync(Guid accountId, CancellationToken ct);
    Task AttachPlayerAsync(Guid accountId, string playerId, string? characterId, CancellationToken ct);
}

public sealed record PlayerIdentityState(bool CharacterExists, string? PlayerId, string? CharacterId);

public sealed class PlayerIdentityGateway(
    IOptions<IntegrationOptions> options,
    IHttpClientFactory httpClientFactory,
    SoulboundLegends.Auth.Api.Data.AuthDbContext db) : IPlayerIdentityGateway
{
    private readonly IntegrationOptions _o = options.Value;

    public async Task<PlayerIdentityState> GetIdentityAsync(Guid accountId, CancellationToken ct)
    {
        var local = await db.PlayerIdentityLinks.FindAsync([accountId], ct);
        if (local != null) return new(true, local.PlayerId, local.CharacterId);

        if (string.IsNullOrWhiteSpace(_o.PlayerDatabaseBaseUrl))
            return new(false, null, null);

        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_o.PlayerDatabaseBaseUrl.TrimEnd('/')}/api/v1/players/by-account/{accountId}", ct);
        if (!response.IsSuccessStatusCode) return new(false, null, null);
        var remote = await response.Content.ReadFromJsonAsync<PlayerIdentityState>(cancellationToken: ct);
        return remote ?? new(false, null, null);
    }

    public async Task AttachPlayerAsync(Guid accountId, string playerId, string? characterId, CancellationToken ct)
    {
        var local = await db.PlayerIdentityLinks.FindAsync([accountId], ct);
        if (local == null)
        {
            db.PlayerIdentityLinks.Add(new SoulboundLegends.Auth.Api.Models.PlayerIdentityLink
            {
                AccountId = accountId, PlayerId = playerId, CharacterId = characterId
            });
        }
        else
        {
            local.PlayerId = playerId;
            local.CharacterId = characterId;
        }
        await db.SaveChangesAsync(ct);
    }
}

public interface ISecurityGateway
{
    Task NotifyAuthenticatedAsync(Guid accountId, Guid sessionId, string? ip, CancellationToken ct);
    Task NotifySuspiciousAsync(Guid? accountId, string reason, string? ip, CancellationToken ct);
}

public sealed class SecurityGateway(IOptions<IntegrationOptions> options, IHttpClientFactory httpClientFactory) : ISecurityGateway
{
    private readonly IntegrationOptions _o = options.Value;
    public async Task NotifyAuthenticatedAsync(Guid accountId, Guid sessionId, string? ip, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_o.AntiCheatBaseUrl)) return;
        var client = httpClientFactory.CreateClient();
        await client.PostAsJsonAsync($"{_o.AntiCheatBaseUrl.TrimEnd('/')}/api/v1/security/authenticated",
            new { accountId, sessionId, ip }, ct);
    }
    public async Task NotifySuspiciousAsync(Guid? accountId, string reason, string? ip, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_o.AntiCheatBaseUrl)) return;
        var client = httpClientFactory.CreateClient();
        await client.PostAsJsonAsync($"{_o.AntiCheatBaseUrl.TrimEnd('/')}/api/v1/security/suspicious",
            new { accountId, reason, ip }, ct);
    }
}
