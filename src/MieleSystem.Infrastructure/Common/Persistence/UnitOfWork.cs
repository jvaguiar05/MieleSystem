using Microsoft.EntityFrameworkCore.Storage;
using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Infrastructure.Common.Persistence;

/// <summary>
/// Implementação padrão de IUnitOfWork usando EF Core.
/// Controla transações e persistência de alterações.
/// </summary>
internal sealed class UnitOfWork(MieleDbContext dbContext)
    : IUnitOfWork,
        IDisposable,
        IAsyncDisposable
{
    private readonly MieleDbContext _dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private IDbContextTransaction? _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("Já existe uma transação ativa.");

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(ct);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        catch
        {
            await RollbackAsync(ct);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _dbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
            await _currentTransaction.DisposeAsync();

        await _dbContext.DisposeAsync();
    }
}
