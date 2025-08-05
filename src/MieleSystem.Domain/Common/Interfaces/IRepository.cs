using MieleSystem.Domain.Common.Base;

namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Contrato genérico para repositórios de entidades.
/// Usa Id interno (int) para lógica interna, e PublicId (Guid) para identidade pública.
/// </summary>
public interface IRepository<T>
    where T : Entity
{
    // Identificador técnico interno (auto-incremento)
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Identificador público (ex: em URLs)
    Task<T?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Remove(T entity);

    // Verificação por Id interno
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);

    // Verificação por PublicId
    Task<bool> ExistsByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
}
