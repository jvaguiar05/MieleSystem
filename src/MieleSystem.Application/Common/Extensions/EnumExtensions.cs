using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Métodos de extensão para enums, como obter DisplayName.
/// </summary>
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        return value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? value.ToString();
    }

    public static TEnum ParseFromString<TEnum>(string value, bool ignoreCase = true)
        where TEnum : struct
    {
        return Enum.TryParse(value, ignoreCase, out TEnum result)
            ? result
            : throw new ArgumentException(
                $"Valor inválido para enum '{typeof(TEnum).Name}': {value}"
            );
    }

    public static IEnumerable<TEnum> GetAllValues<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    }
}
