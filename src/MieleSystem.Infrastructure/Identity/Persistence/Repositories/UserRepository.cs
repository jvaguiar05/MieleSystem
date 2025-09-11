using Microsoft.EntityFrameworkCore;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Infrastructure.Common.Persistence;
using Mail = MieleSystem.Domain.Identity.ValueObjects.Email;

namespace MieleSystem.Infrastructure.Identity.Persistence.Repositories;

/// <summary>
/// Repositório de usuários.
/// Utilizado para acessar e manipular dados de usuários.
/// </summary>
/// <param name="db"></param>
public sealed class UserRepository(MieleDbContext db) : RepositoryBase<User>(db), IUserRepository
{
    /// <summary>
    /// Obtém um usuário pelo e-mail.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="ct"></param>
    /// <returns>Um usuário, se encontrado; caso contrário, null.</returns>
    public async Task<User?> GetByEmailAsync(Mail email, CancellationToken ct = default)
    {
        return await _set.Include(x => x.OtpSessions)
            .Include(x => x.RefreshTokens)
            .Include(x => x.ConnectionLogs)
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    /// <summary>
    /// Verifica se um usuário existe pelo e-mail.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="ct"></param>
    /// <returns>Verdadeiro se o usuário existir, falso caso contrário.</returns>
    public async Task<bool> ExistsByEmailAsync(Mail email, CancellationToken ct = default)
    {
        return await _set.AnyAsync(x => x.Email == email, ct);
    }
}
