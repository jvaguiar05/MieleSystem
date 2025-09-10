using Microsoft.Extensions.Logging;
using MieleSystem.Application.Identity.Services.Email;

namespace MieleSystem.Infrastructure.Identity.Services.Email;

/// <summary>
/// Serviço responsável pelo logging de operações de e-mail.
/// </summary>
public sealed class EmailLoggingService : IEmailLoggingService
{
    private static readonly Action<ILogger, string, string, Exception?> SendingEmail =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(1001, nameof(SendingEmail)),
            "Enviando e-mail '{EmailType}' para {EmailAddress}"
        );

    private static readonly Action<ILogger, string, string, Exception?> EmailSentSuccessfully =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(1002, nameof(EmailSentSuccessfully)),
            "E-mail '{EmailType}' enviado com sucesso para {EmailAddress}"
        );

    private static readonly Action<ILogger, string, string, Exception?> EmailSendFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(1003, nameof(EmailSendFailed)),
            "Falha ao enviar e-mail '{EmailType}' para {EmailAddress}"
        );

    private readonly ILogger<EmailLoggingService> _logger;

    /// <summary>
    /// Inicializa uma nova instância do serviço de logging.
    /// </summary>
    /// <param name="logger">Logger para registro de eventos.</param>
    public EmailLoggingService(ILogger<EmailLoggingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registra o início do envio de um e-mail.
    /// </summary>
    /// <param name="emailType">Tipo do e-mail.</param>
    /// <param name="emailAddress">Endereço de destino.</param>
    public void LogSendingEmail(string emailType, string emailAddress)
    {
        SendingEmail(_logger, emailType, emailAddress, null);
    }

    /// <summary>
    /// Registra o sucesso no envio de um e-mail.
    /// </summary>
    /// <param name="emailType">Tipo do e-mail.</param>
    /// <param name="emailAddress">Endereço de destino.</param>
    public void LogEmailSentSuccessfully(string emailType, string emailAddress)
    {
        EmailSentSuccessfully(_logger, emailType, emailAddress, null);
    }

    /// <summary>
    /// Registra a falha no envio de um e-mail.
    /// </summary>
    /// <param name="emailType">Tipo do e-mail.</param>
    /// <param name="emailAddress">Endereço de destino.</param>
    /// <param name="exception">Exceção que causou a falha.</param>
    public void LogEmailSendFailed(string emailType, string emailAddress, Exception exception)
    {
        EmailSendFailed(_logger, emailType, emailAddress, exception);
    }
}
