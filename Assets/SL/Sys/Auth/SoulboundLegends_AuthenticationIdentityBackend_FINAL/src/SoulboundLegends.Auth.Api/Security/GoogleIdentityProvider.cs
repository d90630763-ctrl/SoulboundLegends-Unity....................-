using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SoulboundLegends.Auth.Api.Configuration;

namespace SoulboundLegends.Auth.Api.Security;

public sealed record VerifiedExternalIdentity(
    string Provider,
    string Subject,
    string? Email,
    bool EmailVerified);

public sealed class GoogleIdentityProvider
{
    private readonly AuthOptions _options;
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

    public GoogleIdentityProvider(IOptions<AuthOptions> options)
    {
        _options = options.Value;
        var retriever = new OpenIdConnectConfigurationRetriever();
        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            "https://accounts.google.com/.well-known/openid-configuration",
            retriever,
            new HttpDocumentRetriever { RequireHttps = true });
    }

    public async Task<VerifiedExternalIdentity> VerifyAsync(string idToken, string nonce, CancellationToken ct)
    {
        if (!_options.GoogleEnabled) throw new InvalidOperationException("Google authentication is disabled.");
        if (string.IsNullOrWhiteSpace(_options.GoogleClientId))
            throw new InvalidOperationException("GoogleClientId is not configured.");

        var config = await _configurationManager.GetConfigurationAsync(ct);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
            ValidateAudience = true,
            ValidAudience = _options.GoogleClientId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = config.SigningKeys,
            ClockSkew = TimeSpan.FromMinutes(2),
            NameClaimType = "sub"
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(idToken, parameters, out _);

        var sub = principal.FindFirstValue("sub");
        var email = principal.FindFirstValue("email");
        var emailVerified = string.Equals(principal.FindFirstValue("email_verified"), "true", StringComparison.OrdinalIgnoreCase);
        var tokenNonce = principal.FindFirstValue("nonce");

        if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(tokenNonce) ||
            !CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(tokenNonce),
                System.Text.Encoding.UTF8.GetBytes(nonce)))
            throw new SecurityTokenException("Google nonce validation failed.");

        if (_options.RequireGoogleEmailVerified && !emailVerified)
            throw new SecurityTokenException("Google account email is not verified.");

        return new VerifiedExternalIdentity("google", sub, email, emailVerified);
    }
}
