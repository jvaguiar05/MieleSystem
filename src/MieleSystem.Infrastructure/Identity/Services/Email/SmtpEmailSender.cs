using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MieleSystem.Application.Identity.Services.Email;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Infrastructure.Identity.Services.Email;

/// <summary>
/// Serviço responsável pelo envio de e-mails via SMTP.
/// </summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private static readonly Action<ILogger, string, Exception?> SmtpClientInitialized =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(1004, nameof(SmtpClientInitialized)),
            "Cliente SMTP inicializado para {SmtpHost}"
        );

    private static readonly Action<ILogger, Exception?> SmtpClientDisposed = LoggerMessage.Define(
        LogLevel.Debug,
        new EventId(1005, nameof(SmtpClientDisposed)),
        "Cliente SMTP descartado"
    );

    private readonly EmailSenderOptions _options;
    private readonly SmtpClient _smtp;
    private readonly ILogger<SmtpEmailSender> _logger;
    private bool _disposed;

    /// <summary>
    /// Inicializa uma nova instância do serviço SMTP.
    /// </summary>
    /// <param name="options">Opções de configuração do SMTP.</param>
    /// <param name="logger">Logger para registro de eventos.</param>
    public SmtpEmailSender(IOptions<EmailSenderOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        EmailConfigurationValidator.ValidateConfiguration(_options);

        _smtp = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword),
            EnableSsl = _options.UseSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 30000, // 30 segundos
        };

        SmtpClientInitialized(_logger, _options.SmtpHost, null);
    }

    /// <summary>
    /// Envia um e-mail usando o cliente SMTP configurado.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino.</param>
    /// <param name="subject">Assunto do e-mail.</param>
    /// <param name="htmlBody">Corpo HTML do e-mail.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <exception cref="ObjectDisposedException">Quando o serviço foi descartado.</exception>
    public async Task SendAsync(
        MieleSystem.Domain.Identity.ValueObjects.Email to,
        string subject,
        string htmlBody,
        CancellationToken ct = default
    )
    {
        ValidateNotDisposed();

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };

        message.To.Add(to.Value);

        _logger.LogDebug(
            "Enviando e-mail via SMTP para {EmailAddress} com assunto '{Subject}'",
            to.Value,
            subject
        );

        await _smtp.SendMailAsync(message, ct);
    }

    /// <summary>
    /// Valida se o serviço não foi descartado.
    /// </summary>
    private void ValidateNotDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SmtpEmailSender));
    }

    /// <summary>
    /// Libera os recursos utilizados pelo serviço.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _smtp?.Dispose();
            SmtpClientDisposed(_logger, null);
            _disposed = true;
        }
    }
}
