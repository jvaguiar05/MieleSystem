using System.Text.RegularExpressions;
using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Exceptions;

namespace MieleSystem.Domain.Identity.ValueObjects;

/// <summary>
/// Value Object que representa um endereço de e-mail válido e normalizado.
/// </summary>
public sealed partial class Email : ValueObject
{
    public string Value { get; }

    private static readonly Regex EmailRegex = MyRegex();

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("E-mail não pode ser vazio.");

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new DomainException($"E-mail inválido: '{value}'");

        Value = normalized;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    // Regex básica para validação (pode ser ajustada conforme necessidade)
    [GeneratedRegex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled,
        "pt-BR"
    )]
    private static partial Regex MyRegex();
}
