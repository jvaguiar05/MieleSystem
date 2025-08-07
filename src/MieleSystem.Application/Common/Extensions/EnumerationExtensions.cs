using MieleSystem.Domain.Common.Base;

namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Extensões para lidar com SmartEnums baseadas em Enumeration.
/// </summary>
public static class EnumerationExtensions
{
    public static T? GetByValue<T>(this IEnumerable<T> list, int value)
        where T : Enumeration
    {
        return list.FirstOrDefault(e => e.Value == value);
    }

    public static T? GetByName<T>(this IEnumerable<T> list, string name)
        where T : Enumeration
    {
        return list.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static bool HasValue<T>(this IEnumerable<T> list, int value)
        where T : Enumeration
    {
        return list.Any(e => e.Value == value);
    }

    public static IEnumerable<string> GetAllNames<T>()
        where T : Enumeration
    {
        return Enumeration.GetAll<T>().Select(e => e.Name);
    }

    public static IEnumerable<int> GetAllValues<T>()
        where T : Enumeration
    {
        return Enumeration.GetAll<T>().Select(e => e.Value);
    }

    public static Dictionary<int, string> ToDictionary<T>()
        where T : Enumeration
    {
        return Enumeration.GetAll<T>().ToDictionary(e => e.Value, e => e.Name);
    }
}
