using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace SoulboundLegends.PlayerDatabaseBackend.Infrastructure;
public sealed class DatabaseOptions { public string ConnectionString { get; set; } = ""; public int CommandTimeoutSeconds { get; set; } = 15; public int MaxRetryAttempts { get; set; } = 3; }
public interface IDatabaseProvider { Task<NpgsqlConnection> OpenAsync(CancellationToken ct); }
public sealed class PostgresDatabaseProvider(IOptions<DatabaseOptions> o) : IDatabaseProvider
{
    readonly DatabaseOptions _o=o.Value;
    public async Task<NpgsqlConnection> OpenAsync(CancellationToken ct) { if(string.IsNullOrWhiteSpace(_o.ConnectionString)) throw new InvalidOperationException("Database connection is not configured."); var c=new NpgsqlConnection(_o.ConnectionString); await c.OpenAsync(ct); return c; }
}
public interface ITransactionRunner { Task<T> RunAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> action, CancellationToken ct); }
public sealed class TransactionRunner(IDatabaseProvider provider, IOptions<DatabaseOptions> opts, ILogger<TransactionRunner> log) : ITransactionRunner
{
    public async Task<T> RunAsync<T>(Func<IDbConnection,IDbTransaction,Task<T>> action,CancellationToken ct)
    {
        Exception? last=null; for(var attempt=1;attempt<=opts.Value.MaxRetryAttempts;attempt++) try { await using var c=await provider.OpenAsync(ct); await using var tx=await c.BeginTransactionAsync(IsolationLevel.ReadCommitted,ct); try { var r=await action(c,tx); await tx.CommitAsync(ct); return r; } catch { await tx.RollbackAsync(ct); throw; } } catch(Exception ex) when(attempt<opts.Value.MaxRetryAttempts && IsTransient(ex)) { last=ex; await Task.Delay(TimeSpan.FromMilliseconds(100*attempt*attempt),ct); log.LogWarning(ex,"Transient database failure on attempt {Attempt}",attempt); } throw last!;
    }
    static bool IsTransient(Exception e)=>e is NpgsqlException or TimeoutException or IOException;
}
public sealed class MigrationRunner(IDatabaseProvider provider, IWebHostEnvironment env, ILogger<MigrationRunner> log)
{
    public async Task RunAsync(CancellationToken ct){ await using var c=await provider.OpenAsync(ct); await c.ExecuteAsync("CREATE TABLE IF NOT EXISTS schema_migrations(version text primary key, checksum text not null, applied_at timestamptz not null default now());"); var dir=Path.Combine(env.ContentRootPath,"..","..","db","migrations"); if(!Directory.Exists(dir)) dir=Path.Combine(env.ContentRootPath,"db","migrations"); foreach(var f in Directory.GetFiles(dir,"*.sql").OrderBy(x=>x)){ var sql=await File.ReadAllTextAsync(f,ct); var version=Path.GetFileNameWithoutExtension(f); var checksum=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(sql))); var old=await c.QuerySingleOrDefaultAsync<string>("select checksum from schema_migrations where version=@version",new{version}); if(old is not null){if(old!=checksum) throw new InvalidOperationException($"Migration checksum mismatch: {version}"); continue;} await using var tx=await c.BeginTransactionAsync(ct); await c.ExecuteAsync(new CommandDefinition(sql,transaction:tx,cancellationToken:ct)); await c.ExecuteAsync("insert into schema_migrations(version,checksum) values(@version,@checksum)",new{version,checksum},tx); await tx.CommitAsync(ct); log.LogInformation("Applied migration {Version}",version);} }
    public async Task<object> VerifyAsync(CancellationToken ct){ await using var c=await provider.OpenAsync(ct); return new { database=await c.ExecuteScalarAsync<int>("select 1") == 1, migrationTable=await c.ExecuteScalarAsync<int>("select count(*) from schema_migrations") }; }
}
public sealed class DatabaseHealthService(IDatabaseProvider p){ public async Task<object> CheckAsync(CancellationToken ct){await using var c=await p.OpenAsync(ct); var one=await c.ExecuteScalarAsync<int>("select 1"); return new{status=one==1?"healthy":"unhealthy"};}}
public sealed class DatabaseHealthCheck(DatabaseHealthService s): Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck { public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext c,CancellationToken ct=default){try{await s.CheckAsync(ct);return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();}catch(Exception e){return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database unavailable",e);}}}
