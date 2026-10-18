namespace SoulboundLegends.Auth.Api.Configuration;

public sealed class AuthOptions
{
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
    public int SessionTimeoutMinutes { get; set; } = 60;
    public int ReconnectGraceSeconds { get; set; } = 120;
    public int MaxSessionsPerAccount { get; set; } = 3;
    public string DuplicateSessionPolicy { get; set; } = "Reject";
    public bool PasswordLoginEnabled { get; set; } = true;
    public bool GoogleEnabled { get; set; } = true;
    public string GoogleClientId { get; set; } = "";
    public bool RequireGoogleEmailVerified { get; set; } = true;
    public int MaxLoginAttemptsPerWindow { get; set; } = 8;
    public int LoginRateWindowSeconds { get; set; } = 300;
    public int LockoutMinutes { get; set; } = 15;
    public bool RefreshReplayRevokesSession { get; set; } = true;
    public string JwtPrivateKeyPemPath { get; set; } = "";
    public string JwtKeyId { get; set; } = "sbl-auth-1";
    public int TrustedProxyCount { get; set; }
}
public sealed class RecoveryOptions
{
    public bool Enabled { get; set; } = true;
    public int TokenLifetimeMinutes { get; set; } = 30;
    public string DeliveryWebhookUrl { get; set; } = "";
}
public sealed class IntegrationOptions
{
    public string PlayerDatabaseBaseUrl { get; set; } = "";
    public string ServerAuthorityBaseUrl { get; set; } = "";
    public string AntiCheatBaseUrl { get; set; } = "";
    public string AnalyticsBaseUrl { get; set; } = "";
    public string MasterRegistryBaseUrl { get; set; } = "";
}
