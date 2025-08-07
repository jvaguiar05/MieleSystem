namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Interface para gerenciamento de unidade de trabalho.
/// Define métodos para controle de transações e persistência de alterações.
/// </summary>
public interface IUnitOfWork
{
    // Inicia uma transação manualmente (opcional, se não usar ambient transaction)
    Task BeginTransactionAsync(CancellationToken ct = default);

    // Efetiva todas as alterações pendentes
    Task CommitAsync(CancellationToken ct = default);

    // Desfaz alterações (quando transação aberta)
    Task RollbackAsync(CancellationToken ct = default);

    // Verifica se há transação ativa
    bool HasActiveTransaction { get; }
}
