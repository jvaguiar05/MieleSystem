using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Infrastructure.Common.Persistence;

namespace MieleSystem.Infrastructure.Identity.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de logs de auditoria de usuários.
/// Responsável por persistir logs imutáveis no contexto Identity.
/// </summary>
public sealed class UserAuditLogRepository(MieleDbContext db) : IUserAuditLogRepository
{
    private readonly MieleDbContext _db = db;

    /// <summary>
    /// Adiciona um log de auditoria de usuário.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task AddAsync(UserAuditLog log, CancellationToken ct = default)
    {
        await _db.UserAuditLogs.AddAsync(log, ct);
    }
}
