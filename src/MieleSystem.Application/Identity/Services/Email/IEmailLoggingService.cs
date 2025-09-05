namespace MieleSystem.Application.Identity.Services.Email;

/// <summary>
/// Interface para logging de operações de e-mail.
/// </summary>
public interface IEmailLoggingService
{
    /// <summary>
    /// Registra o início do envio de um e-mail.
    /// </summary>
    /// <param name="emailType">Tipo do e-mail.</param>
    /// <param name="emailAddress">Endereço de destino.</param>
    void LogSendingEmail(string emailType, string emailAddress);

    /// <summary>
    /// Registra o sucesso no envio de um e-mail.
    /// </summary>
    /// <param name="emailType">Tipo do e-mail.</param>
    /// <param name="emailAddress">Endereço de destino.</param>
    void LogEmailSentSuccessfully(string emailType, string emailAddress);

    /// <summary>
    /// Registra a falha no envio de um e-mail.
    /// </summary>
    /// <param name="emailType">Tipo do e-mail.</param>
    /// <param name="emailAddress">Endereço de destino.</param>
    /// <param name="exception">Exceção que causou a falha.</param>
    void LogEmailSendFailed(string emailType, string emailAddress, Exception exception);
}
