using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Repositories;

/// <summary>
/// Repositório especializado para operações com a entidade User.
/// Estende o contrato genérico de repositório com métodos específicos do contexto Identity.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    // Busca um usuário pelo e-mail.
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    // Verifica se um e-mail já está registrado.
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);

    // Obtém todos os usuários com status de registro pendente.
    Task<IReadOnlyList<User>> GetPendingRegistrationsAsync(
        CancellationToken cancellationToken = default
    );

    // Obtém todos os usuários do tipo Viewer com data de expiração vencida.
    Task<IReadOnlyList<User>> GetExpiredViewerAccountsAsync(
        DateOnly referenceDate,
        CancellationToken cancellationToken = default
    );
}
