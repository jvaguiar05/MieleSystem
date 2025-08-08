using Microsoft.EntityFrameworkCore;
using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Infrastructure.Common.Persistence;

internal abstract class RepositoryBase<T>(MieleDbContext db) : IRepository<T>
    where T : Entity
{
    protected readonly MieleDbContext _db = db;
    protected readonly DbSet<T> _set = db.Set<T>();

    // =========================
    // Consultas (respeitam filtros globais, ex.: soft delete)
    // =========================

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        // NÃO usar FindAsync: ele ignora filtros globais
        return await _set.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public virtual async Task<T?> GetByPublicIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await _set.FirstOrDefaultAsync(e => e.PublicId == publicId, ct);
    }

    public virtual async Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default)
    {
        return await _set.AnyAsync(e => e.Id == id, ct);
    }

    public virtual async Task<bool> ExistsByPublicIdAsync(
        Guid publicId,
        CancellationToken ct = default
    )
    {
        return await _set.AnyAsync(e => e.PublicId == publicId, ct);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        // Persistência efetiva é responsabilidade do UnitOfWork.CommitAsync()
    }

    // =========================
    // Helpers para repositórios específicos
    // =========================

    /// <summary>
    /// Query padrão (com tracking). Útil para construir consultas com Includes nos repositórios concretos.
    /// </summary>
    protected IQueryable<T> Query() => _set;

    /// <summary>
    /// Query sem tracking (ideal p/ leitura pura).
    /// </summary>
    protected IQueryable<T> QueryNoTracking() => _set.AsNoTracking();
}
