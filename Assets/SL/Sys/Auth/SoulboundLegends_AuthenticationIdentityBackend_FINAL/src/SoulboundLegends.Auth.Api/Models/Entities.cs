namespace SoulboundLegends.Auth.Api.Models;

public enum AccountStatus { Active, Locked, Suspended, RecoveryLocked }

public sealed class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NormalizedEmail { get; set; } = "";
    public string? DisplayEmail { get; set; }
    public string? PasswordHash { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTimeOffset? LockoutUntil { get; set; }
    public string Roles { get; set; } = "Player";
}

public sealed class ExternalIdentity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public string Provider { get; set; } = "";
    public string ProviderSubject { get; set; } = "";
    public string? EmailAtProvider { get; set; }
    public DateTimeOffset LinkedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastVerifiedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class AuthSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public string DeviceId { get; set; } = "";
    public string? IpAddress { get; set; }
    public string? UserAgentHash { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastActivityAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset ReconnectGraceUntil { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public string CurrentRefreshTokenHash { get; set; } = "";
    public long RefreshGeneration { get; set; }
}

public sealed class RefreshTokenRecord
{
    public long Id { get; set; }
    public Guid SessionId { get; set; }
    public string TokenHash { get; set; } = "";
    public long Generation { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ConsumedAt { get; set; }
}

public sealed class SecurityEvent
{
    public long Id { get; set; }
    public Guid? AccountId { get; set; }
    public string Type { get; set; } = "";
    public string? IpAddress { get; set; }
    public string? MetadataJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class RecoveryChallenge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public string TokenHash { get; set; } = "";
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class PlayerIdentityLink
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public string PlayerId { get; set; } = "";
    public string? CharacterId { get; set; }
    public DateTimeOffset LinkedAt { get; set; } = DateTimeOffset.UtcNow;
}
