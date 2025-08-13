using Microsoft.EntityFrameworkCore;
using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Infrastructure.Common.Persistence;

/// <summary>
/// Implementação base do repositório de escrita.
/// Opera sempre respeitando filtros globais (ex.: soft delete).
/// </summary>
internal abstract class RepositoryBase<T>(MieleDbContext db) : IRepository<T>
    where T : Entity
{
    protected readonly MieleDbContext _db = db;
    protected readonly DbSet<T> _set = db.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        // NÃO usar FindAsync: ele ignora filtros globais (soft delete, etc.)
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
    }

    public virtual void Update(T entity)
    {
        _set.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.Delete();
            _set.Update(entity);
        }
        else
        {
            _set.Remove(entity);
        }
    }
}
