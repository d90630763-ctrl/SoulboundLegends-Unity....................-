using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using SoulboundLegends.PlayerDatabaseBackend;
using SoulboundLegends.PlayerDatabaseBackend.Infrastructure;
using SoulboundLegends.PlayerDatabaseBackend.Security;
using SoulboundLegends.PlayerDatabaseBackend.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("SLDB_");
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));
builder.Services.AddSingleton<IDatabaseProvider, PostgresDatabaseProvider>();
builder.Services.AddSingleton<MigrationRunner>();
builder.Services.AddSingleton<DatabaseHealthService>();
builder.Services.AddScoped<ITransactionRunner, TransactionRunner>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<MutationService>();
builder.Services.AddScoped<WorldStateService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<EconomyService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<SupremeRuinService>();
builder.Services.AddScoped<IAuthorityContext, JwtAuthorityContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.Authority = builder.Configuration["Security:Authority"];
    o.Audience = builder.Configuration["Security:Audience"];
    o.RequireHttpsMetadata = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(o => o.AddPolicy("admin", p => p.RequireRole("admin", "operations")));
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
var app = builder.Build();
app.UseExceptionHandler(errorApp => errorApp.Run(async ctx =>
{
    var feature = ctx.Features.Get<IExceptionHandlerFeature>();
    var ex = feature?.Error;
    ctx.Response.StatusCode = ex is ConcurrencyException ? 409 : ex is UnauthorizedAccessException ? 403 : 500;
    ctx.Response.ContentType = "application/problem+json";
    await ctx.Response.WriteAsJsonAsync(new { title = "Request failed", status = ctx.Response.StatusCode, detail = app.Environment.IsDevelopment() ? ex?.Message : "The operation could not be completed." });
}));
app.UseAuthentication(); app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapGet("/health/ready", async (DatabaseHealthService h, CancellationToken ct) => Results.Ok(await h.CheckAsync(ct)));
app.MapGet("/api/v1/player/{playerId:guid}", async (Guid playerId, PlayerRepository repo, IAuthorityContext auth, CancellationToken ct) =>
{
    auth.RequirePlayer(playerId);
    var p = await repo.GetSnapshotAsync(playerId, ct);
    return p is null ? Results.NotFound() : Results.Ok(p);
}).RequireAuthorization();
app.MapPost("/api/v1/player/{playerId:guid}/mutation", async (Guid playerId, MutationRequest request, MutationService svc, IAuthorityContext auth, CancellationToken ct) =>
{
    auth.RequirePlayer(playerId);
    return Results.Ok(await svc.ApplyAsync(playerId, request, auth, ct));
}).RequireAuthorization();
app.MapPost("/api/v1/player/{playerId:guid}/economy", async (Guid playerId, EconomyMutation request, EconomyService svc, IAuthorityContext auth, CancellationToken ct) => { auth.RequirePlayer(playerId); var balance=await svc.ApplyAsync(playerId,request.Currency,request.Delta,request.Reason,request.IdempotencyKey,request.ExpectedVersion,auth,ct); return Results.Ok(new { balance }); }).RequireAuthorization();
app.MapPost("/api/v1/admin/ruins/{ruinId:guid}/claim", async (Guid ruinId, Guid playerId, string idempotencyKey, SupremeRuinService svc, IAuthorityContext auth, CancellationToken ct) => { auth.RequireAdmin(); await svc.ClaimInheritanceAsync(ruinId,playerId,auth,idempotencyKey,ct); return Results.Ok(); }).RequireAuthorization("admin");
app.MapGet("/api/v1/admin/player/{playerId:guid}", async (Guid playerId, AdminService svc, IAuthorityContext auth, CancellationToken ct) => Results.Ok(await svc.InspectPlayerAsync(playerId, auth, ct))).RequireAuthorization("admin");
app.MapGet("/api/v1/admin/transactions/{playerId:guid}", async (Guid playerId, AdminService svc, IAuthorityContext auth, CancellationToken ct) => Results.Ok(await svc.InspectTransactionsAsync(playerId, auth, ct))).RequireAuthorization("admin");
app.MapGet("/api/v1/admin/world/rifts/{riftId:guid}", async (Guid riftId, AdminService svc, IAuthorityContext auth, CancellationToken ct) => Results.Ok(await svc.InspectRiftAsync(riftId, auth, ct))).RequireAuthorization("admin");
app.MapGet("/api/v1/admin/world/ruins/{ruinId:guid}", async (Guid ruinId, AdminService svc, IAuthorityContext auth, CancellationToken ct) => Results.Ok(await svc.InspectRuinAsync(ruinId, auth, ct))).RequireAuthorization("admin");
app.MapPost("/api/v1/admin/migrations/verify", async (MigrationRunner r, IAuthorityContext auth, CancellationToken ct) => { auth.RequireAdmin(); return Results.Ok(await r.VerifyAsync(ct)); }).RequireAuthorization("admin");
if (app.Environment.IsDevelopment()) app.MapOpenApi();
await app.Services.GetRequiredService<MigrationRunner>().RunAsync(CancellationToken.None);
await app.RunAsync();
