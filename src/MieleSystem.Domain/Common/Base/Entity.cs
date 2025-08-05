namespace MieleSystem.Domain.Common.Base;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Uma entidade é definida por sua identidade (Id), e não pelos valores de suas propriedades.
/// </summary>
public abstract class Entity
{
    /// Identificador único da entidade.
    public Guid Id { get; protected set; }

    /// Construtor protegido que força a definição do Id ao criar uma entidade.
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// Compara duas entidades pelo Id. Se os Ids forem iguais, considera-se que representam a mesma entidade no domínio.
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true; // mesmo objeto na memória

        if (obj is null || obj.GetType() != GetType())
            return false; // tipos diferentes ou objeto nulo

        return Id == ((Entity)obj).Id;
    }

    /// Retorna um código de hash baseado no Id.
    /// Necessário para o uso correto em coleções baseadas em hash (ex: HashSet, Dictionary).
    public override int GetHashCode() => Id.GetHashCode();

    /// Operador de igualdade baseado em Equals.
    public static bool operator ==(Entity? a, Entity? b) =>
        a is null && b is null || a is not null && a.Equals(b);

    /// Operador de diferença.
    public static bool operator !=(Entity? a, Entity? b) => !(a == b);
}
