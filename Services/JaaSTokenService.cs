using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Abstractions;
using Shared.JaaS;
using Shared.VideoModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace Services;

/// <summary>
/// Generates RS256-signed JWTs for Jitsi as a Service (JaaS / 8x8.vc).
/// The private key never leaves the server.
/// </summary>
public class JaaSTokenService : IJaaSTokenService
{
    private readonly JaaSOptions _options;

    public JaaSTokenService(IOptions<JaaSOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public JoinLiveSessionResponseDTO GenerateToken(
        string userId,
        string userName,
        string email,
        string roomName,
        bool isModerator)
    {
        // ── Load RSA private key from PEM ──────────────────────────────────────
        using var rsa = RSA.Create();

        // Strip PEM headers/footers and all whitespace to get raw base64
        var pemBase64 = _options.PrivateKeyPem
            .Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "");

        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(pemBase64), out _);

        // Clone into a non-disposed RSA instance for the security key
        var rsaClone = RSA.Create();
        rsaClone.ImportParameters(rsa.ExportParameters(includePrivateParameters: true));

        var securityKey = new RsaSecurityKey(rsaClone) { KeyId = _options.ApiKeyId };
        var signingCreds = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        // ── Build JWT header ───────────────────────────────────────────────────
        var header = new JwtHeader(signingCreds);

        // ── Build JWT payload ──────────────────────────────────────────────────
        var now    = DateTime.UtcNow;
        var expiry = now.AddHours(1);

        // The "context" claim must be a nested object — serialize manually so
        // the JwtSecurityToken handler embeds it correctly.
        var contextObj = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["id"]        = userId,
                ["name"]      = userName,
                ["email"]     = email,
                ["moderator"] = isModerator ? "true" : "false"  // JaaS requires string, not bool
            },
            ["features"] = new Dictionary<string, object>
            {
                ["livestreaming"]   = "true",
                ["recording"]       = "true",
                ["transcription"]   = "false",
                ["outbound-call"]   = "false"   // hyphenated key — must use Dictionary
            }
        };
        var contextJson = JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(contextObj))!;

        var payload = new JwtPayload
        {
            { JwtRegisteredClaimNames.Iss, "chat" },          // JaaS requires fixed value "chat"
            { JwtRegisteredClaimNames.Aud, "jitsi" },
            { JwtRegisteredClaimNames.Sub, _options.AppId },  // sub = AppId (tenant), not domain
            { "room",    roomName },
            { JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds() },
            { JwtRegisteredClaimNames.Nbf, new DateTimeOffset(now).ToUnixTimeSeconds() },
            { JwtRegisteredClaimNames.Exp, new DateTimeOffset(expiry).ToUnixTimeSeconds() },
            { "context", contextJson }
        };

        var token = new JwtSecurityToken(header, payload);
        var jwt   = new JwtSecurityTokenHandler().WriteToken(token);

        // ── Build room URL ─────────────────────────────────────────────────────
        var roomUrl = $"https://{_options.Domain}/{_options.AppId}/{roomName}";

        return new JoinLiveSessionResponseDTO { Jwt = jwt, RoomUrl = roomUrl };
    }
}
