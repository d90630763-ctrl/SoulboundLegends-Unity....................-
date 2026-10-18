using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SoulboundLegends.Auth.Api.Configuration;
using SoulboundLegends.Auth.Api.Data;
using SoulboundLegends.Auth.Api.Integration;
using SoulboundLegends.Auth.Api.Models;
using SoulboundLegends.Auth.Api.Security;

namespace SoulboundLegends.Auth.Api.Services;

public interface IAuthenticationService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ip, string? userAgent, CancellationToken ct);
    Task<AuthResponse> LoginPasswordAsync(PasswordLoginRequest request, string? ip, string? userAgent, CancellationToken ct);
    Task<AuthResponse> LoginGoogleAsync(GoogleLoginRequest request, string? ip, string? userAgent, CancellationToken ct);
    Task<AuthResponse> RefreshAsync(RefreshRequest request, string? ip, string? userAgent, CancellationToken ct);
    Task LogoutAsync(Guid accountId, Guid sessionId, bool allDevices, CancellationToken ct);
    Task<SessionInfo?> GetSessionAsync(Guid accountId, Guid sessionId, CancellationToken ct);
    Task<IReadOnlyList<ExternalIdentityInfo>> GetExternalIdentitiesAsync(Guid accountId, CancellationToken ct);
    Task LinkGoogleAsync(Guid accountId, LinkProviderRequest request, CancellationToken ct);
    Task UnlinkProviderAsync(Guid accountId, string provider, CancellationToken ct);
}

public sealed class AuthenticationService(
    AuthDbContext db,
    PasswordHasher passwordHasher,
    JwtTokenService jwt,
    GoogleIdentityProvider google,
    IPlayerIdentityGateway playerIdentity,
    ISecurityGateway security,
    IOptions<AuthOptions> options) : IAuthenticationService
{
    private readonly AuthOptions _o = options.Value;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        if (!_o.PasswordLoginEnabled) throw new InvalidOperationException("Password authentication is disabled.");
        if (string.IsNullOrWhiteSpace(request.DeviceId)) throw new ArgumentException("DeviceId is required.");
        if (await db.Accounts.AnyAsync(x => x.NormalizedEmail == email, ct))
            throw new InvalidOperationException("Account already exists.");

        var account = new Account
        {
            NormalizedEmail = email,
            DisplayEmail = request.Email.Trim(),
            PasswordHash = passwordHasher.Hash(request.Password)
        };
        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);
        await Audit(account.Id, "account.created", ip, null, ct);
        return await CreateSessionAsync(account, request.DeviceId, ip, userAgent, ct);
    }

    public async Task<AuthResponse> LoginPasswordAsync(PasswordLoginRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var account = await db.Accounts.SingleOrDefaultAsync(x => x.NormalizedEmail == email, ct);
        if (account == null)
        {
            await Audit(null, "login.failed.unknown_account", ip, null, ct);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (account.Status == AccountStatus.Suspended)
            throw new UnauthorizedAccessException("Account is suspended.");
        if (account.LockoutUntil > DateTimeOffset.UtcNow)
            throw new UnauthorizedAccessException("Account temporarily locked.");

        if (account.PasswordHash == null || !passwordHasher.Verify(request.Password, account.PasswordHash))
        {
            account.FailedLoginCount++;
            if (account.FailedLoginCount >= _o.MaxLoginAttemptsPerWindow)
            {
                account.Status = AccountStatus.Locked;
                account.LockoutUntil = DateTimeOffset.UtcNow.AddMinutes(_o.LockoutMinutes);
            }
            await db.SaveChangesAsync(ct);
            await Audit(account.Id, "login.failed.password", ip, null, ct);
            await security.NotifySuspiciousAsync(account.Id, "password-login-failure", ip, ct);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        account.FailedLoginCount = 0;
        account.LockoutUntil = null;
        account.Status = AccountStatus.Active;
        account.LastLoginAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        await Audit(account.Id, "login.success.password", ip, null, ct);
        return await CreateSessionAsync(account, request.DeviceId, ip, userAgent, ct);
    }

    public async Task<AuthResponse> LoginGoogleAsync(GoogleLoginRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var verified = await google.VerifyAsync(request.IdToken, request.Nonce, ct);
        var identity = await db.ExternalIdentities.Include(x => x.Account)
            .SingleOrDefaultAsync(x => x.Provider == verified.Provider && x.ProviderSubject == verified.Subject, ct);

        Account account;
        if (identity == null)
        {
            var existingByEmail = !string.IsNullOrWhiteSpace(verified.Email)
                ? await db.Accounts.SingleOrDefaultAsync(x => x.NormalizedEmail == NormalizeEmail(verified.Email), ct)
                : null;
            if (existingByEmail != null)
                throw new InvalidOperationException("An account already exists for this email. Sign in with its existing authentication method, then link Google from account settings.");

            account = new Account
            {
                DisplayEmail = verified.Email,
                NormalizedEmail = NormalizeEmail(verified.Email ?? $"{verified.Subject}@google.invalid")
            };
            db.Accounts.Add(account);
            await db.SaveChangesAsync(ct);
            db.ExternalIdentities.Add(new ExternalIdentity
            {
                AccountId = account.Id,
                Provider = verified.Provider,
                ProviderSubject = verified.Subject,
                EmailAtProvider = verified.Email
            });
            await db.SaveChangesAsync(ct);
            await Audit(account.Id, "account.created.google", ip, null, ct);
        }
        else
        {
            account = identity.Account;
            identity.LastVerifiedAt = DateTimeOffset.UtcNow;
            identity.EmailAtProvider = verified.Email;
            if (account.Status == AccountStatus.Suspended) throw new UnauthorizedAccessException("Account is suspended.");
            account.LastLoginAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
            await Audit(account.Id, "login.success.google", ip, null, ct);
        }

        return await CreateSessionAsync(account, request.DeviceId, ip, userAgent, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var hash = JwtTokenService.HashRefreshToken(request.RefreshToken);
        var session = await db.Sessions.SingleOrDefaultAsync(x => x.CurrentRefreshTokenHash == hash, ct);
        if (session == null)
        {
            var replay = await db.RefreshTokenRecords.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
            if (replay != null)
            {
                var replaySession = await db.Sessions.SingleOrDefaultAsync(x => x.Id == replay.SessionId, ct);
                if (replaySession != null && replaySession.RevokedAt == null && _o.RefreshReplayRevokesSession)
                {
                    replaySession.RevokedAt = DateTimeOffset.UtcNow;
                    replaySession.RevocationReason = "refresh-token-replay";
                    db.SecurityEvents.Add(new SecurityEvent { AccountId = replaySession.AccountId, Type = "security.refresh_replay", IpAddress = ip });
                    await db.SaveChangesAsync(ct);
                }
            }
            throw new UnauthorizedAccessException("Refresh token is invalid.");
        }

        if (session.RevokedAt != null || session.ExpiresAt <= DateTimeOffset.UtcNow)
            throw new UnauthorizedAccessException("Session is invalid.");

        var account = await db.Accounts.SingleAsync(x => x.Id == session.AccountId, ct);
        if (account.Status != AccountStatus.Active)
            throw new UnauthorizedAccessException("Account is not active.");

        var oldHash = session.CurrentRefreshTokenHash;
        var newRefresh = JwtTokenService.NewRefreshToken();
        session.CurrentRefreshTokenHash = JwtTokenService.HashRefreshToken(newRefresh);
        session.RefreshGeneration++;
        db.RefreshTokenRecords.Add(new RefreshTokenRecord
        {
            SessionId = session.Id,
            TokenHash = oldHash,
            Generation = session.RefreshGeneration - 1,
            ConsumedAt = DateTimeOffset.UtcNow
        });
        db.RefreshTokenRecords.Add(new RefreshTokenRecord
        {
            SessionId = session.Id,
            TokenHash = session.CurrentRefreshTokenHash,
            Generation = session.RefreshGeneration
        });
        session.LastActivityAt = DateTimeOffset.UtcNow;
        session.ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_o.SessionTimeoutMinutes);
        session.ReconnectGraceUntil = DateTimeOffset.UtcNow.AddSeconds(_o.ReconnectGraceSeconds);
        await db.SaveChangesAsync(ct);
        await Audit(account.Id, "token.refresh", ip, null, ct);

        var access = jwt.CreateAccessToken(account, session);
        var identity = await playerIdentity.GetIdentityAsync(account.Id, ct);
        return new AuthResponse(account.Id, session.Id, access.Token, newRefresh, access.ExpiresAt, session.ExpiresAt,
            identity.CharacterExists, identity.PlayerId, identity.CharacterId);
    }

    public async Task LogoutAsync(Guid accountId, Guid sessionId, bool allDevices, CancellationToken ct)
    {
        var sessions = allDevices
            ? await db.Sessions.Where(x => x.AccountId == accountId && x.RevokedAt == null).ToListAsync(ct)
            : await db.Sessions.Where(x => x.AccountId == accountId && x.Id == sessionId && x.RevokedAt == null).ToListAsync(ct);

        foreach (var s in sessions)
        {
            s.RevokedAt = DateTimeOffset.UtcNow;
            s.RevocationReason = allDevices ? "logout-all" : "logout";
        }
        await db.SaveChangesAsync(ct);
        await Audit(accountId, allDevices ? "logout.all" : "logout", null, null, ct);
    }

    public async Task<SessionInfo?> GetSessionAsync(Guid accountId, Guid sessionId, CancellationToken ct)
    {
        var s = await db.Sessions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == sessionId && x.AccountId == accountId, ct);
        return s == null ? null : new SessionInfo(s.Id, s.AccountId, s.DeviceId, s.CreatedAt, s.LastActivityAt, s.ExpiresAt, s.RevokedAt != null);
    }

    public async Task<IReadOnlyList<ExternalIdentityInfo>> GetExternalIdentitiesAsync(Guid accountId, CancellationToken ct) =>
        await db.ExternalIdentities.AsNoTracking().Where(x => x.AccountId == accountId)
            .Select(x => new ExternalIdentityInfo(x.Provider, x.ProviderSubject, x.LinkedAt)).ToListAsync(ct);

    public async Task LinkGoogleAsync(Guid accountId, LinkProviderRequest request, CancellationToken ct)
    {
        var verified = await google.VerifyAsync(request.IdToken, request.Nonce, ct);
        var existing = await db.ExternalIdentities.SingleOrDefaultAsync(x =>
            x.Provider == verified.Provider && x.ProviderSubject == verified.Subject, ct);

        if (existing != null && existing.AccountId != accountId)
            throw new InvalidOperationException("External identity is already linked to another account.");

        if (existing == null)
        {
            db.ExternalIdentities.Add(new ExternalIdentity
            {
                AccountId = accountId,
                Provider = verified.Provider,
                ProviderSubject = verified.Subject,
                EmailAtProvider = verified.Email
            });
        }
        await db.SaveChangesAsync(ct);
        await Audit(accountId, "identity.link.google", null, null, ct);
    }

    public async Task UnlinkProviderAsync(Guid accountId, string provider, CancellationToken ct)
    {
        var account = await db.Accounts.SingleAsync(x => x.Id == accountId, ct);
        var identities = await db.ExternalIdentities.Where(x => x.AccountId == accountId && x.Provider == provider).ToListAsync(ct);
        if (identities.Count == 0) throw new KeyNotFoundException("Provider identity not linked.");

        var remainingAuth = !string.IsNullOrWhiteSpace(account.PasswordHash) ||
                            await db.ExternalIdentities.AnyAsync(x => x.AccountId == accountId && x.Provider != provider, ct);
        if (!remainingAuth) throw new InvalidOperationException("Cannot unlink the last authentication method.");

        db.ExternalIdentities.RemoveRange(identities);
        await db.SaveChangesAsync(ct);
        await Audit(accountId, "identity.unlink." + provider, null, null, ct);
    }

    private async Task<AuthResponse> CreateSessionAsync(Account account, string deviceId, string? ip, string? userAgent, CancellationToken ct)
    {
        var active = await db.Sessions.Where(x => x.AccountId == account.Id && x.RevokedAt == null && x.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderBy(x => x.CreatedAt).ToListAsync(ct);

        var policy = _o.DuplicateSessionPolicy.Trim();
        if (policy.Equals("Reject", StringComparison.OrdinalIgnoreCase) && active.Any(x => x.DeviceId == deviceId))
            throw new InvalidOperationException("A session for this device already exists.");

        if (policy.Equals("Replace", StringComparison.OrdinalIgnoreCase) && active.Any(x => x.DeviceId == deviceId))
        {
            foreach (var s in active.Where(x => x.DeviceId == deviceId))
            {
                s.RevokedAt = DateTimeOffset.UtcNow;
                s.RevocationReason = "duplicate-session-replaced";
            }
        }

        var remaining = active.Where(x => x.DeviceId != deviceId || !policy.Equals("Replace", StringComparison.OrdinalIgnoreCase)).ToList();
        if (remaining.Count >= _o.MaxSessionsPerAccount)
        {
            var oldest = remaining.FirstOrDefault();
            if (oldest != null)
            {
                oldest.RevokedAt = DateTimeOffset.UtcNow;
                oldest.RevocationReason = "maximum-sessions";
            }
        }

        var refresh = JwtTokenService.NewRefreshToken();
        var session = new AuthSession
        {
            AccountId = account.Id,
            DeviceId = deviceId,
            IpAddress = ip,
            UserAgentHash = HashUserAgent(userAgent),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_o.SessionTimeoutMinutes),
            ReconnectGraceUntil = DateTimeOffset.UtcNow.AddSeconds(_o.ReconnectGraceSeconds),
            CurrentRefreshTokenHash = JwtTokenService.HashRefreshToken(refresh)
        };
        db.Sessions.Add(session);
        await db.SaveChangesAsync(ct);

        await security.NotifyAuthenticatedAsync(account.Id, session.Id, ip, ct);
        var access = jwt.CreateAccessToken(account, session);
        var identity = await playerIdentity.GetIdentityAsync(account.Id, ct);
        return new AuthResponse(account.Id, session.Id, access.Token, refresh, access.ExpiresAt, session.ExpiresAt,
            identity.CharacterExists, identity.PlayerId, identity.CharacterId);
    }

    private async Task Audit(Guid? accountId, string type, string? ip, string? metadata, CancellationToken ct)
    {
        db.SecurityEvents.Add(new SecurityEvent { AccountId = accountId, Type = type, IpAddress = ip, MetadataJson = metadata });
        await db.SaveChangesAsync(ct);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();
    private static string? HashUserAgent(string? ua) => ua == null ? null : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(ua)));
}
