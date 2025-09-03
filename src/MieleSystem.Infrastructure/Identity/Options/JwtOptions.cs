using System.Text;

namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    // Propriedade para receber o valor numérico (ex: 180)
    public int AccessTokenExpirationInMinutes { get; set; }

    // Propriedade calculada para uso no código
    public TimeSpan AccessTokenExpiration => TimeSpan.FromMinutes(AccessTokenExpirationInMinutes);

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

        // Validando a nova propriedade
        if (AccessTokenExpirationInMinutes <= 0)
            throw new InvalidOperationException(
                "Security:Jwt:AccessTokenExpirationInMinutes deve ser maior que zero."
            );
    }
}
