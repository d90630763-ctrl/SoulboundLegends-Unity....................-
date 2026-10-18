using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SoulboundLegends.Auth.Api.Configuration;
using SoulboundLegends.Auth.Api.Models;

namespace SoulboundLegends.Auth.Api.Security;

public sealed class JwtTokenService
{
    private readonly RsaSigningKeyProvider _keys;
    private readonly AuthOptions _options;

    public JwtTokenService(RsaSigningKeyProvider keys, Microsoft.Extensions.Options.IOptions<AuthOptions> options)
    {
        _keys = keys;
        _options = options.Value;
    }

    public (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(Account account, AuthSession session)
    {
        var now = DateTimeOffset.UtcNow;
        var exp = now.AddMinutes(_options.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("sid", session.Id.ToString()),
            new("roles", account.Roles)
        };
        foreach (var role in account.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var creds = new SigningCredentials(_keys.SecurityKey, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims,
            now.UtcDateTime, exp.UtcDateTime, creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), exp);
    }

    public static string NewRefreshToken() =>
        Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

    public static string HashRefreshToken(string token)
    {
        var hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
