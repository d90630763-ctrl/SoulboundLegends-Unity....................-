using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoulboundLegends.Auth.Api.Data;
using SoulboundLegends.Auth.Api.Integration;
using SoulboundLegends.Auth.Api.Security;
using SoulboundLegends.Auth.Api.Services;
using SoulboundLegends.Auth.Api.Configuration;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Authentication"));
builder.Services.Configure<RecoveryOptions>(builder.Configuration.GetSection("Recovery"));
builder.Services.Configure<IntegrationOptions>(builder.Configuration.GetSection("Integration"));

builder.Services.AddDbContext<AuthDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 12,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddSingleton<RsaSigningKeyProvider>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<GoogleIdentityProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ISystemConnectionRegistry, SystemConnectionRegistry>();
builder.Services.AddScoped<IPlayerIdentityGateway, PlayerIdentityGateway>();
builder.Services.AddScoped<ISecurityGateway, SecurityGateway>();
builder.Services.AddScoped<IRecoveryService, RecoveryService>();
builder.Services.AddScoped<AdminService>();

var keyProvider = new RsaSigningKeyProvider(builder.Configuration);
var signingCredentials = new SigningCredentials(keyProvider.SecurityKey, SecurityAlgorithms.RsaSha256);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Authentication:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = keyProvider.SecurityKey,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "sub"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var sessionId = context.Principal?.FindFirst("sid")?.Value;
                var accountId = context.Principal?.FindFirst("sub")?.Value;
                if (string.IsNullOrWhiteSpace(sessionId) || string.IsNullOrWhiteSpace(accountId))
                {
                    context.Fail("Missing authenticated identity claims.");
                    return;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<AuthDbContext>();
                var session = await db.Sessions.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == Guid.Parse(sessionId) && x.AccountId == Guid.Parse(accountId));

                if (session is null || session.RevokedAt != null || session.ExpiresAt <= DateTimeOffset.UtcNow)
                {
                    context.Fail("Session is not valid.");
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;
        var status = ex switch
        {
            SoulboundLegends.Auth.Api.Controllers.AuthenticationHttpException a => a.Status,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            Microsoft.IdentityModel.Tokens.SecurityTokenException => StatusCodes.Status401Unauthorized,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status409Conflict,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = status == 500 ? "internal_server_error" : "authentication_error", message = status == 500 ? "The authentication service could not complete the request." : ex?.Message });
    });
});
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "soulbound-auth" }));
app.MapGet("/.well-known/jwks.json", (RsaSigningKeyProvider keys) =>
    Results.Json(keys.GetJwkDocument()));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    if (app.Environment.IsDevelopment())
        await db.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program { }
