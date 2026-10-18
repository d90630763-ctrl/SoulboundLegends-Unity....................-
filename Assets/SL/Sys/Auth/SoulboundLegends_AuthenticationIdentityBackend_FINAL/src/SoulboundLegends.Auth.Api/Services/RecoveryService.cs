using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using SoulboundLegends.Auth.Api.Configuration;
using SoulboundLegends.Auth.Api.Data;
using SoulboundLegends.Auth.Api.Models;
using SoulboundLegends.Auth.Api.Security;

namespace SoulboundLegends.Auth.Api.Services;

public interface IRecoveryService
{
    Task RequestAsync(RecoveryRequest request, CancellationToken ct);
    Task CompleteAsync(CompleteRecoveryRequest request, CancellationToken ct);
}

public sealed class RecoveryService(
    AuthDbContext db,
    PasswordHasher hasher,
    IHttpClientFactory httpClientFactory,
    IOptions<RecoveryOptions> options) : IRecoveryService
{
    private readonly RecoveryOptions _o = options.Value;

    public async Task RequestAsync(RecoveryRequest request, CancellationToken ct)
    {
        if (!_o.Enabled) return;
        var email = request.Email.Trim().ToUpperInvariant();
        var account = await db.Accounts.SingleOrDefaultAsync(x => x.NormalizedEmail == email, ct);
        if (account == null) return;

        var raw = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(48));
        db.RecoveryChallenges.Add(new RecoveryChallenge
        {
            AccountId = account.Id,
            TokenHash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw))),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_o.TokenLifetimeMinutes)
        });
        await db.SaveChangesAsync(ct);

        if (string.IsNullOrWhiteSpace(_o.DeliveryWebhookUrl))
            throw new InvalidOperationException("Recovery is enabled but no recovery delivery webhook is configured.");

        var client = httpClientFactory.CreateClient();
        await client.PostAsJsonAsync(_o.DeliveryWebhookUrl, new { accountId = account.Id, recoveryToken = raw }, ct);
    }

    public async Task CompleteAsync(CompleteRecoveryRequest request, CancellationToken ct)
    {
        var hash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(request.Token)));
        var challenge = await db.RecoveryChallenges.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (challenge == null || challenge.UsedAt != null || challenge.ExpiresAt <= DateTimeOffset.UtcNow)
            throw new UnauthorizedAccessException("Recovery token is invalid.");

        var account = await db.Accounts.SingleAsync(x => x.Id == challenge.AccountId, ct);
        account.PasswordHash = hasher.Hash(request.NewPassword);
        account.Status = AccountStatus.Active;
        account.LockoutUntil = null;
        account.FailedLoginCount = 0;

        var sessions = await db.Sessions.Where(x => x.AccountId == account.Id && x.RevokedAt == null).ToListAsync(ct);
        foreach (var s in sessions)
        {
            s.RevokedAt = DateTimeOffset.UtcNow;
            s.RevocationReason = "account-recovery";
        }

        challenge.UsedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
