namespace MieleSystem.Domain.Common.Base;

/// <summary>
/// Objeto de valor do domínio: definido por seus valores, não por identidade.
/// Imutável por definição e comparado exclusivamente com base em seus componentes.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Retorna os componentes que definem a igualdade entre Value Objects.
    /// Deve retornar sempre na mesma ordem para objetos equivalentes.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        // HashCode.Combine é mais performático e reduz colisões
        return GetEqualityComponents()
            .Aggregate(default(int), (hash, component) => HashCode.Combine(hash, component));
    }

    public static bool operator ==(ValueObject? a, ValueObject? b) =>
        a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
}
