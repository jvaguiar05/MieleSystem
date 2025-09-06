using System.Text;

namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Security:Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    // Propriedade para receber o valor numérico (ex: 60)
    public int AccessTokenExpirationInMinutes { get; set; }

    // Propriedade para receber o valor numérico (ex: 7)
    public int RefreshTokenExpirationInDays { get; set; }

    // Propriedade calculada para uso no código
    public TimeSpan AccessTokenExpiration => TimeSpan.FromMinutes(AccessTokenExpirationInMinutes);

    // Propriedade calculada para uso no código
    public TimeSpan RefreshTokenExpiration => TimeSpan.FromDays(RefreshTokenExpirationInDays);

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Secret) || Encoding.UTF8.GetBytes(Secret).Length < 32)
            throw new InvalidOperationException(
                "Security:Jwt:Secret deve ter pelo menos 32 caracteres."
            );

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("Security:Jwt:Issuer não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("Security:Jwt:Audience não pode ser vazio.");

        // Validando as propriedades de expiração
        if (AccessTokenExpirationInMinutes <= 0)
            throw new InvalidOperationException(
                "Security:Jwt:AccessTokenExpirationInMinutes deve ser maior que zero."
            );

        if (RefreshTokenExpirationInDays <= 0)
            throw new InvalidOperationException(
                "Security:Jwt:RefreshTokenExpirationInDays deve ser maior que zero."
            );
    }
}
