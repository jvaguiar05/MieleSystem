using MieleSystem.Domain.Common.Base;

namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Contrato genérico para repositórios de entidades (Write Model).
/// Focado apenas em operações necessárias para mutação de agregados.
/// </summary>
public interface IRepository<T>
    where T : Entity
{
    /// <summary>
    /// Obtém uma entidade pelo identificador interno (auto-incremento).
    /// Usado para operações internas de escrita.
    /// </summary>
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Obtém uma entidade pelo identificador público (seguro para exposição externa).
    /// Usado quando o comando vem de fora do domínio.
    /// </summary>
    Task<T?> GetByPublicIdAsync(Guid publicId, CancellationToken ct = default);

    /// <summary>
    /// Adiciona uma nova entidade ao repositório.
    /// A persistência efetiva é responsabilidade do UnitOfWork.
    /// </summary>
    Task AddAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Marca uma entidade como atualizada para persistência.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Marca uma entidade para remoção.
    /// Se implementar <see cref="ISoftDeletable"/>, aplica soft delete.
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Verifica se existe uma entidade com o identificador interno informado.
    /// </summary>
    Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Verifica se existe uma entidade com o identificador público informado.
    /// </summary>
    Task<bool> ExistsByPublicIdAsync(Guid publicId, CancellationToken ct = default);
}
