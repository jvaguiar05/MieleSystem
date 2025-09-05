using Mail = MieleSystem.Domain.Identity.ValueObjects.Email;

namespace MieleSystem.Application.Identity.Services.Email;

/// <summary>
/// Interface para validação de dados relacionados a e-mails.
/// </summary>
public interface IEmailValidator
{
    /// <summary>
    /// Valida o endereço de e-mail.
    /// </summary>
    /// <param name="email">Endereço de e-mail a ser validado.</param>
    /// <exception cref="ArgumentNullException">Quando o e-mail é null.</exception>
    /// <exception cref="ArgumentException">Quando o e-mail é inválido.</exception>
    void ValidateEmailAddress(Mail email);

    /// <summary>
    /// Valida o nome do usuário.
    /// </summary>
    /// <param name="userName">Nome do usuário a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o nome é inválido.</exception>
    void ValidateUserName(string userName);

    /// <summary>
    /// Valida o código OTP.
    /// </summary>
    /// <param name="code">Código OTP a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o código é inválido.</exception>
    void ValidateOtpCode(string code);

    /// <summary>
    /// Valida o tempo de expiração do OTP.
    /// </summary>
    /// <param name="expiresAtUtc">Tempo de expiração a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o tempo é inválido.</exception>
    void ValidateExpirationTime(DateTime expiresAtUtc);

    /// <summary>
    /// Valida o tempo de alteração da senha.
    /// </summary>
    /// <param name="changedAtUtc">Tempo de alteração a ser validado.</param>
    /// <exception cref="ArgumentException">Quando o tempo é inválido.</exception>
    void ValidateChangedTime(DateTime changedAtUtc);
}
