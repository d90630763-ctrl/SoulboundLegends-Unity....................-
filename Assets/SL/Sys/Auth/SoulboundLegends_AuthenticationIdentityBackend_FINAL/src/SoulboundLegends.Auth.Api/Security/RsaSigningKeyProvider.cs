using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;
using SoulboundLegends.Auth.Api.Configuration;

namespace SoulboundLegends.Auth.Api.Security;

public sealed class RsaSigningKeyProvider
{
    public SecurityKey SecurityKey { get; }
    public Rsa Rsa { get; }
    public string KeyId { get; }

    public RsaSigningKeyProvider(IConfiguration config)
    {
        KeyId = config["Authentication:JwtKeyId"] ?? "sbl-auth-1";
        var path = config["Authentication:JwtPrivateKeyPemPath"];
        Rsa = RSA.Create();

        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            Rsa.ImportFromPem(File.ReadAllText(path));
        }
        else if (Environment.GetEnvironmentVariable("SBL_JWT_PRIVATE_KEY_PEM") is { Length: > 0 } pem)
        {
            Rsa.ImportFromPem(pem);
        }
        else
        {
            throw new InvalidOperationException("A JWT RSA private key must be supplied through SBL_JWT_PRIVATE_KEY_PEM or Authentication:JwtPrivateKeyPemPath.");
        }

        SecurityKey = new RsaSecurityKey(Rsa) { KeyId = KeyId };
    }

    public object GetJwkDocument()
    {
        var p = Rsa.ExportParameters(false);
        return new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    use = "sig",
                    alg = "RS256",
                    kid = KeyId,
                    n = Base64UrlEncoder.Encode(p.Modulus!),
                    e = Base64UrlEncoder.Encode(p.Exponent!)
                }
            }
        };
    }
}
