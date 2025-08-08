namespace MieleSystem.Domain.Common.Utils;

/// <summary>
/// Marca uma entidade como deletável logicamente.
/// Permite que a entidade seja "deletada" sem ser removida fisicamente do banco de dados.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }

    void Delete();
    void Restore();
}
