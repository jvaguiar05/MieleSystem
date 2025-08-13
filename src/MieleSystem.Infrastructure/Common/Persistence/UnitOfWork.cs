using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using MieleSystem.Application.Common.Events;
using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Infrastructure.Common.Persistence;

/// <summary>
/// Implementação padrão de <see cref="IUnitOfWork"/> usando EF Core.
/// Controla transações, persistência de alterações e publicação de Domain Events.
/// </summary>
internal sealed class UnitOfWork(MieleDbContext dbContext, IMediator mediator)
    : IUnitOfWork,
        IDisposable,
        IAsyncDisposable
{
    private readonly MieleDbContext _dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IMediator _mediator =
        mediator ?? throw new ArgumentNullException(nameof(mediator));
    private IDbContextTransaction? _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await _dbContext.SaveChangesAsync(ct);
        await DispatchDomainEventsAsync(ct);

        return result;
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("Já existe uma transação ativa.");

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        try
        {
            await SaveChangesAsync(ct);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(ct);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        catch
        {
            await RollbackTransactionAsync(ct);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        var entitiesWithEvents = _dbContext
            .ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var events = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        // Limpa eventos para evitar reprocessamento
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in events)
        {
            // Cria o tipo genérico EventNotification<TEvent>
            var notificationType = typeof(EventNotification<>).MakeGenericType(
                domainEvent.GetType()
            );

            // Cria instância do notification com o evento como argumento
            var notificationInstance = Activator.CreateInstance(notificationType, domainEvent)!;

            // Publica via MediatR
            await _mediator.Publish((INotification)notificationInstance, ct);
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
