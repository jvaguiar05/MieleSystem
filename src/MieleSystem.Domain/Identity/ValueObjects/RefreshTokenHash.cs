using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Exceptions;

namespace MieleSystem.Domain.Identity.ValueObjects;

/// <summary>
/// Value Object que representa um hash de refresh token (SHA-256).
/// </summary>
public sealed class RefreshTokenHash : ValueObject
{
    public string Value { get; }

    public RefreshTokenHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("O hash do refresh token não pode ser vazio.");

        // Um hash SHA-256 em formato hexadecimal tem sempre 64 caracteres.
        // Esta é uma excelente validação para garantir a integridade.
        if (value.Length != 64)
            throw new DomainException("Hash de refresh token parece inválido.");

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(RefreshTokenHash hash) => hash.Value;
}
