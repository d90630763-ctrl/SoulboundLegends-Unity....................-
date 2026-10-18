namespace SoulboundLegends.Auth.Api.Models;

public sealed record RegisterRequest(string Email, string Password, string DeviceId);
public sealed record PasswordLoginRequest(string Email, string Password, string DeviceId);
public sealed record GoogleLoginRequest(string IdToken, string Nonce, string DeviceId);
public sealed record RefreshRequest(string RefreshToken, string DeviceId);
public sealed record LinkProviderRequest(string IdToken, string Nonce);
public sealed record LogoutRequest(bool AllDevices = false);
public sealed record RecoveryRequest(string Email);
public sealed record CompleteRecoveryRequest(string Token, string NewPassword);

public sealed record AuthResponse(
    Guid AccountId,
    Guid SessionId,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset SessionExpiresAt,
    bool CharacterExists,
    string? PlayerId,
    string? CharacterId);

public sealed record SessionInfo(
    Guid SessionId,
    Guid AccountId,
    string DeviceId,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastActivityAt,
    DateTimeOffset ExpiresAt,
    bool Revoked);

public sealed record ExternalIdentityInfo(string Provider, string ProviderSubject, DateTimeOffset LinkedAt);
