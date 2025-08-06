using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Exceptions;

namespace MieleSystem.Domain.Identity.ValueObjects;

/// <summary>
/// Value Object que representa um hash de senha.
/// Esse valor já deve ter sido gerado externamente (ex: por um IPasswordHasher).
/// </summary>
public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    public PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("O hash da senha não pode ser vazio.");

        // Regras adicionais de segurança, se necessário:
        // ex: comprimento mínimo de hash (ex: BCrypt = 60 caracteres)
        if (value.Length < 60)
            throw new DomainException("Hash de senha parece inválido.");

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PasswordHash hash) => hash.Value;
}
