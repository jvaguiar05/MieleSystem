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
    /// <summary>
    /// Obtém um usuário pelo e-mail.
    /// Retorna null se não encontrado.
    /// </summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);

    /// <summary>
    /// Verifica se existe um usuário com o e-mail informado.
    /// </summary>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
}
