using Microsoft.Extensions.Options;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Infrastructure.Identity.Services.Domain;

/// <summary>
/// Implementação concreta do serviço de hash de senhas utilizando BCrypt.
/// Utiliza um fator de trabalho configurável para balancear segurança e performance.
/// </summary>
public sealed class PasswordHasher(IOptions<BCryptOptions> options) : IPasswordHasher
{
    private readonly int _workFactor = options.Value.WorkFactor;

    public string Hash(string plainTextPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainTextPassword, _workFactor);
    }

    public bool Verify(string hashedPassword, string plainTextPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
    }

    public bool NeedsRehash(string hashedPassword)
    {
        return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, _workFactor);
    }
}
