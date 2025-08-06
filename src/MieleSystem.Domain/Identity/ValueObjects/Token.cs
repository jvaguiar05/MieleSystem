using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Exceptions;

namespace MieleSystem.Domain.Identity.ValueObjects;

public sealed class Token : ValueObject
{
    public string Value { get; }

    public Token(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Token não pode ser nulo ou vazio.");

        if (value.Length < 32)
            throw new DomainException("Token inválido ou muito curto.");

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Token token) => token.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
