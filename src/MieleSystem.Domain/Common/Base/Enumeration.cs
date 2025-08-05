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

    public int CompareTo(object? other) => other is Enumeration e ? Value.CompareTo(e.Value) : -1;

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
