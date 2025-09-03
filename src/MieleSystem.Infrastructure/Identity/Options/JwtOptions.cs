using System.Text;

namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public TimeSpan AccessTokenExpiration { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Secret) || Encoding.UTF8.GetBytes(Secret).Length < 32)
            throw new InvalidOperationException("JWT_SECRET deve ter pelo menos 32 caracteres.");

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JWT_ISSUER não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT_AUDIENCE não pode ser vazio.");

        if (AccessTokenExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException(
                "JWT_ACCESS_TOKEN_EXPIRATION deve ser maior que zero."
            );
    }
}
