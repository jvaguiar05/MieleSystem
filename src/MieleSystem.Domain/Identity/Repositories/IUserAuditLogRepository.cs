using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Domain.Identity.Repositories;

/// <summary>
/// Repositório para persistência de logs de auditoria de usuários.
/// Responsável apenas por criação (logs são imutáveis).
/// </summary>
public interface IUserAuditLogRepository
{
    /// <summary>
    /// Adiciona um novo log ao sistema.
    /// </summary>
    Task AddAsync(UserAuditLog log, CancellationToken ct = default);
}
