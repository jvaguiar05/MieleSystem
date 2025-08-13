namespace MieleSystem.Domain.Common.Base;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Define tanto o identificador interno (Id) quanto o identificador público (PublicId).
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Construtor protegido para criação controlada.
    /// </summary>
    /// <param name="publicId">
    /// Identificador público da entidade.
    /// Caso seja Guid.Empty, um novo Guid será gerado automaticamente.
    /// </param>
    protected Entity(Guid publicId)
    {
        PublicId = publicId == Guid.Empty ? Guid.NewGuid() : publicId;
    }

    /// <summary>
    /// Construtor sem parâmetros protegido para uso do EF Core e serializadores.
    /// </summary>
    protected Entity()
        : this(Guid.Empty) { }

    /// <summary>
    /// Identificador interno técnico da entidade. Gerado pelo banco de dados (auto-incremento).
    /// Não deve ser exposto em APIs.
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Identificador público da entidade. Seguro para exposição externa.
    /// </summary>
    public Guid PublicId { get; protected init; }

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
