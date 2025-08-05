namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Interface para gerenciamento de unidade de trabalho.
/// Coordena repositórios e transações.
/// </summary>
public interface IUnitOfWork
{
    // Inicia uma transação manualmente (opcional, se não usar ambient transaction)
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    // Efetiva todas as alterações pendentes
    Task CommitAsync(CancellationToken cancellationToken = default);

    // Desfaz alterações (quando transação aberta)
    Task RollbackAsync(CancellationToken cancellationToken = default);

    // Verifica se há transação ativa
    bool HasActiveTransaction { get; }

    // Exemplo de repositórios que o UnitOfWork coordena
    // IClientRepository Clients { get; }
    // IUserRepository Users { get; }
    // Outros repositórios podem ser adicionados conforme necessário
}
