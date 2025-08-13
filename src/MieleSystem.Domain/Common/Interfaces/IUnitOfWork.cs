namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Interface para gerenciamento de unidade de trabalho.
/// Responsável por controlar persistência de alterações e, opcionalmente, transações.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persiste todas as alterações pendentes no contexto atual.
    /// Também aciona a publicação de Domain Events antes do commit.
    /// </summary>
    /// <returns>Número de registros afetados.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Inicia uma transação manualmente.
    /// Use apenas quando precisar agrupar múltiplas operações em uma única transação explícita.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Confirma a transação manual atualmente ativa.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Reverte a transação manual atualmente ativa.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Indica se há transação ativa.
    /// </summary>
    bool HasActiveTransaction { get; }
}
