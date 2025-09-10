using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Infrastructure.Identity.Services.Domain;

/// <summary>
/// Implementação baseada em código numérico randômico de 6 dígitos.
/// </summary>
public sealed class OtpService(IOptions<OtpOptions> options) : IOtpService
{
    private readonly OtpOptions _options = options.Value;

    public OtpCode Generate()
    {
        var code = GenerateSixDigitCode();
        var expiresAt = DateTime.UtcNow.AddSeconds(_options.ExpirationSeconds);

        return new OtpCode(code, expiresAt);
    }

    public bool Validate(OtpCode expected, string provided)
    {
        return !expected.IsExpired() && expected.Matches(provided);
    }

    private static string GenerateSixDigitCode()
    {
        var number = RandomNumberGenerator.GetInt32(100_000, 1_000_000);
        return number.ToString();
    }
}
