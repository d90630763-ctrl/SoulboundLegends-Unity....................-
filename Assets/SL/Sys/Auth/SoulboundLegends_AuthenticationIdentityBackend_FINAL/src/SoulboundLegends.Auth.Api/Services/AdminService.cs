using Microsoft.EntityFrameworkCore;
using SoulboundLegends.Auth.Api.Data;
using SoulboundLegends.Auth.Api.Models;

namespace SoulboundLegends.Auth.Api.Services;

public sealed class AdminService(AuthDbContext db)
{
    public Task<Account?> FindAccountAsync(Guid id, CancellationToken ct) =>
        db.Accounts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<AuthSession>> SessionsAsync(Guid accountId, CancellationToken ct) =>
        db.Sessions.AsNoTracking().Where(x => x.AccountId == accountId).OrderByDescending(x => x.CreatedAt).ToListAsync(ct);

    public async Task ForceLogoutAsync(Guid accountId, string reason, Guid adminAccountId, CancellationToken ct)
    {
        var sessions = await db.Sessions.Where(x => x.AccountId == accountId && x.RevokedAt == null).ToListAsync(ct);
        foreach (var s in sessions)
        {
            s.RevokedAt = DateTimeOffset.UtcNow;
            s.RevocationReason = reason;
        }
        db.SecurityEvents.Add(new SecurityEvent { AccountId = accountId, Type = "admin.force_logout", MetadataJson = $"admin={adminAccountId};reason={reason}" });
        await db.SaveChangesAsync(ct);
    }

    public async Task SetSuspendedAsync(Guid accountId, bool suspended, Guid adminAccountId, CancellationToken ct)
    {
        var account = await db.Accounts.SingleAsync(x => x.Id == accountId, ct);
        account.Status = suspended ? AccountStatus.Suspended : AccountStatus.Active;
        db.SecurityEvents.Add(new SecurityEvent
        {
            AccountId = accountId,
            Type = suspended ? "admin.account_suspended" : "admin.account_unsuspended",
            MetadataJson = $"admin={adminAccountId}"
        });
        await db.SaveChangesAsync(ct);
    }

    public Task<List<SecurityEvent>> SecurityEventsAsync(Guid accountId, CancellationToken ct) =>
        db.SecurityEvents.AsNoTracking().Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.CreatedAt).Take(500).ToListAsync(ct);
}
