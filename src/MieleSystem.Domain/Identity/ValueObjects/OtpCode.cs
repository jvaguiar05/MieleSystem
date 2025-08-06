using System.Text.RegularExpressions;
using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Exceptions;

namespace MieleSystem.Domain.Identity.ValueObjects;

/// <summary>
/// Representa um código OTP (One-Time Password) com expiração.
/// Usado para autenticação de dois fatores, verificação de identidade, etc.
/// </summary>
public sealed partial class OtpCode : ValueObject
{
    public string Code { get; }
    public DateTime ExpiresAt { get; }

    private static readonly Regex Numeric6Digits = MyRegex();

    public OtpCode(string code, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Código OTP não pode ser vazio.");

        if (!Numeric6Digits.IsMatch(code))
            throw new DomainException("Código OTP deve conter exatamente 6 dígitos numéricos.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Código OTP já está expirado.");

        Code = code;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public bool Matches(string input) => input == Code;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return ExpiresAt;
    }

    public override string ToString() => Code;

    // Regex para validar códigos OTP de 6 dígitos numéricos
    [GeneratedRegex(@"^\d{6}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
