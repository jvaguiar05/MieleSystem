namespace MieleSystem.Domain.Common.Base;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Define tanto o identificador interno (Id) quanto o identificador público (PublicId).
/// </summary>
public abstract class Entity
{
    /// Identificador interno técnico da entidade. Gerado pelo banco de dados (auto-incremento).
    /// Não deve ser exposto em APIs.
    public int Id { get; protected set; }

    /// Identificador público da entidade. Seguro para exposição externa.
    public Guid PublicId { get; protected set; }

    /// Construtor para criação de nova entidade.
    /// Gera automaticamente o PublicId.
    protected Entity()
    {
        PublicId = Guid.NewGuid();
    }

    /// Construtor para reidratação via ORM (por exemplo, EF Core).
    protected Entity(Guid publicId)
    {
        PublicId = publicId == Guid.Empty ? Guid.NewGuid() : publicId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null || obj.GetType() != GetType())
            return false;

        return PublicId == ((Entity)obj).PublicId;
    }

    public override int GetHashCode() => PublicId.GetHashCode();

    public static bool operator ==(Entity? a, Entity? b) =>
        a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(Entity? a, Entity? b) => !(a == b);
}
