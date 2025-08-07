namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Métodos auxiliares para manipulação e normalização de strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Verifica se a string é nula, vazia ou contém apenas espaços em branco.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Remove espaços duplicados e normaliza o espaçamento.
    /// </summary>
    public static string NormalizeWhitespace(this string value)
    {
        return string.Join(
            " ",
            value.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
        );
    }

    /// <summary>
    /// Transforma a string em um slug básico (ex: nome-de-usuario).
    /// </summary>
    public static string ToSlug(this string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace(".", "-")
            .Replace("@", "-")
            .Replace("--", "-");
    }

    /// <summary>
    /// Limpa espaços, converte para minúsculas e remove caracteres especiais simples.
    /// </summary>
    public static string NormalizeSafe(this string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "")
            .Replace("@", "")
            .Replace(".", "");
    }
}
