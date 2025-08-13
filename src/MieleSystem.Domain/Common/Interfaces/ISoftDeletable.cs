namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Marca uma entidade como deletável logicamente.
/// Permite que a entidade seja "deletada" sem ser removida fisicamente do banco de dados.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }

    void Delete();
    void Restore();
}
