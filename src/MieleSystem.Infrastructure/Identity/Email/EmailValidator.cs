using MieleSystem.Application.Identity.Services.Email;

namespace MieleSystem.Infrastructure.Identity.Email;

/// <summary>
/// Serviço responsável pela validação de dados relacionados a e-mails.
/// </summary>
public sealed class EmailValidator : IEmailValidator
{
    /// <summary>
    /// Valida o endereço de e-mail.
    /// </summary>
    /// <param name="email">Endereço de e-mail a ser validado.</param>
    /// <exception cref="ArgumentNullException">Quando o e-mail é null.</exception>
    /// <exception cref="ArgumentException">Quando o e-mail é inválido.</exception>
    public void ValidateEmailAddress(MieleSystem.Domain.Identity.ValueObjects.Email email)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (string.IsNullOrWhiteSpace(email.Value))
            throw new ArgumentException(
                "Endereço de e-mail não pode ser nulo ou vazio",
                nameof(email)
            );
    }

    /// <summary>
    /// Valida o nome do usuário.
    /// </summary>
    /// <param name="userName">Nome do usuário a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o nome é inválido.</exception>
    public void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException(
                "Nome do usuário não pode ser nulo ou vazio",
                nameof(userName)
            );
    }

    /// <summary>
    /// Valida o código OTP.
    /// </summary>
    /// <param name="code">Código OTP a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o código é inválido.</exception>
    public void ValidateOtpCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código OTP não pode ser nulo ou vazio", nameof(code));
    }

    /// <summary>
    /// Valida o tempo de expiração do OTP.
    /// </summary>
    /// <param name="expiresAtUtc">Tempo de expiração a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o tempo é inválido.</exception>
    public void ValidateExpirationTime(DateTime expiresAtUtc)
    {
        if (expiresAtUtc <= DateTime.UtcNow)
            throw new ArgumentException(
                "Tempo de expiração deve ser no futuro",
                nameof(expiresAtUtc)
            );
    }

    /// <summary>
    /// Valida o tempo de alteração da senha.
    /// </summary>
    /// <param name="changedAtUtc">Tempo de alteração a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o tempo é inválido.</exception>
    public void ValidateChangedTime(DateTime changedAtUtc)
    {
        if (changedAtUtc > DateTime.UtcNow)
            throw new ArgumentException(
                "Tempo de alteração não pode ser no futuro",
                nameof(changedAtUtc)
            );
    }
}
