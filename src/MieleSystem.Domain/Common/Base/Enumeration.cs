using MieleSystem.Domain.Common.Exceptions;

namespace MieleSystem.Domain.Common.Base;

/// <summary>
/// Implementação do padrão Smart Enum (Enumeration Class),
/// que permite representar constantes nomeadas com comportamento, lógica e metadados associados.
/// </summary>
public abstract class Enumeration(int value, string name) : IComparable
{
    public int Value { get; } = value;
    public string Name { get; } = name;

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        return Value == otherValue.Value && GetType() == obj.GetType();
    }

    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Compara esta instância com outra do mesmo tipo.
    /// </summary>
    /// <exception cref="BusinessRuleValidationException">
    /// Lançada quando o objeto comparado não é uma Enumeration do mesmo tipo.
    /// </exception>
    public int CompareTo(object? other)
    {
        if (other is not Enumeration e)
            throw new BusinessRuleValidationException(
                $"O objeto comparado não é uma instância válida de {GetType().Name}."
            );

        return Value.CompareTo(e.Value);
    }

    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration
    {
        return typeof(T)
            .GetFields(
                System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.Static
                    | System.Reflection.BindingFlags.DeclaredOnly
            )
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    public static bool operator ==(Enumeration? a, Enumeration? b) =>
        a is null && b is null || a is not null && a.Equals(b);

    public static bool operator !=(Enumeration? a, Enumeration? b) => !(a == b);
}
