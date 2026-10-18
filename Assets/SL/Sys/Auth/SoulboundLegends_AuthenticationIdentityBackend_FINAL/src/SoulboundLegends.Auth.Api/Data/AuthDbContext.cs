using Microsoft.EntityFrameworkCore;
using SoulboundLegends.Auth.Api.Models;

namespace SoulboundLegends.Auth.Api.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();
    public DbSet<AuthSession> Sessions => Set<AuthSession>();
    public DbSet<SecurityEvent> SecurityEvents => Set<SecurityEvent>();
    public DbSet<RefreshTokenRecord> RefreshTokenRecords => Set<RefreshTokenRecord>();
    public DbSet<RecoveryChallenge> RecoveryChallenges => Set<RecoveryChallenge>();
    public DbSet<PlayerIdentityLink> PlayerIdentityLinks => Set<PlayerIdentityLink>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Account>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.NormalizedEmail).IsUnique();
            e.Property(x => x.Status).HasMaxLength(32);
            e.Property(x => x.NormalizedEmail).HasMaxLength(320);
        });
        b.Entity<ExternalIdentity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Provider, x.ProviderSubject }).IsUnique();
            e.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);
        });
        b.Entity<AuthSession>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.AccountId);
            e.HasIndex(x => x.RefreshTokenHash).IsUnique();
            e.Property(x => x.DeviceId).HasMaxLength(256);
        });
        b.Entity<RefreshTokenRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasIndex(x => x.SessionId);
        });
        b.Entity<SecurityEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.AccountId);
            e.HasIndex(x => x.CreatedAt);
        });
        b.Entity<RecoveryChallenge>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.TokenHash).IsUnique();
        });
        b.Entity<PlayerIdentityLink>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.AccountId).IsUnique();
            e.HasIndex(x => x.PlayerId).IsUnique();
        });
    }
}
