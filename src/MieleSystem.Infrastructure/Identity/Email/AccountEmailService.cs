using Microsoft.Extensions.Logging;
using MieleSystem.Application.Identity.Services.Email;
using EmailObject = MieleSystem.Domain.Identity.ValueObjects.Email;

namespace MieleSystem.Infrastructure.Identity.Email;

/// <summary>
/// Orquestrador responsável pelo envio de e-mails relacionados a operações de conta.
/// Coordena validação, renderização de templates, logging e envio de e-mails.
/// </summary>
public sealed class AccountEmailService : IAccountEmailService, IDisposable
{
    private static class EmailTypes
    {
        public const string Welcome = "Welcome";
        public const string Otp = "OTP";
        public const string PasswordChanged = "PasswordChanged";
    }

    private static class EmailSubjects
    {
        public const string Welcome = "Bem-vindo ao MieleSystem!";
        public const string Otp = "Código de verificação - MieleSystem";
        public const string PasswordChanged = "Sua senha foi alterada";
    }

    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _templateService;
    private readonly IEmailLoggingService _loggingService;
    private readonly IEmailValidator _emailValidator;
    private readonly ILogger<AccountEmailService> _logger;
    private bool _disposed;

    /// <summary>
    /// Inicializa uma nova instância do orquestrador de e-mails.
    /// </summary>
    /// <param name="emailSender">Serviço de envio de e-mails.</param>
    /// <param name="templateService">Serviço de renderização de templates.</param>
    /// <param name="loggingService">Serviço de logging.</param>
    /// <param name="emailValidator">Serviço de validação de e-mails.</param>
    /// <param name="logger">Logger para registro de eventos.</param>
    public AccountEmailService(
        IEmailSender emailSender,
        IEmailTemplateService templateService,
        IEmailLoggingService loggingService,
        IEmailValidator emailValidator,
        ILogger<AccountEmailService> logger
    )
    {
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        _templateService =
            templateService ?? throw new ArgumentNullException(nameof(templateService));
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _emailValidator = emailValidator ?? throw new ArgumentNullException(nameof(emailValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Envia um e-mail de boas-vindas para um novo usuário.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino.</param>
    /// <param name="userName">Nome de exibição do usuário.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <exception cref="ArgumentNullException">Quando 'to' ou 'userName' são null ou vazios.</exception>
    /// <exception cref="ObjectDisposedException">Quando o serviço foi descartado.</exception>
    public async Task SendWelcomeAsync(
        EmailObject to,
        string userName,
        CancellationToken ct = default
    )
    {
        ValidateNotDisposed();
        _emailValidator.ValidateEmailAddress(to);
        _emailValidator.ValidateUserName(userName);

        try
        {
            _loggingService.LogSendingEmail(EmailTypes.Welcome, to.Value);

            var subject = EmailSubjects.Welcome;
            var body = await _templateService.RenderWelcomeTemplateAsync(userName);

            await _emailSender.SendAsync(to, subject, body, ct);

            _loggingService.LogEmailSentSuccessfully(EmailTypes.Welcome, to.Value);
        }
        catch (Exception ex)
        {
            _loggingService.LogEmailSendFailed(EmailTypes.Welcome, to.Value, ex);
            throw;
        }
    }

    /// <summary>
    /// Envia um e-mail contendo código OTP para autenticação de dois fatores.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino.</param>
    /// <param name="code">Código OTP a ser enviado.</param>
    /// <param name="expiresAtUtc">Data e hora de expiração do código (em UTC).</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <exception cref="ArgumentNullException">Quando 'to' ou 'code' são null ou vazios.</exception>
    /// <exception cref="ArgumentException">Quando 'expiresAtUtc' é no passado.</exception>
    /// <exception cref="ObjectDisposedException">Quando o serviço foi descartado.</exception>
    public async Task SendOtpAsync(
        EmailObject to,
        string code,
        DateTime expiresAtUtc,
        CancellationToken ct = default
    )
    {
        ValidateNotDisposed();
        _emailValidator.ValidateEmailAddress(to);
        _emailValidator.ValidateOtpCode(code);
        _emailValidator.ValidateExpirationTime(expiresAtUtc);

        try
        {
            _loggingService.LogSendingEmail(EmailTypes.Otp, to.Value);

            var subject = EmailSubjects.Otp;
            var body = await _templateService.RenderOtpTemplateAsync(code, expiresAtUtc);

            await _emailSender.SendAsync(to, subject, body, ct);

            _loggingService.LogEmailSentSuccessfully(EmailTypes.Otp, to.Value);
        }
        catch (Exception ex)
        {
            _loggingService.LogEmailSendFailed(EmailTypes.Otp, to.Value, ex);
            throw;
        }
    }

    /// <summary>
    /// Envia um e-mail notificando alteração de senha na conta.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino.</param>
    /// <param name="changedAtUtc">Data e hora da alteração (em UTC).</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <exception cref="ArgumentNullException">Quando 'to' é null.</exception>
    /// <exception cref="ArgumentException">Quando 'changedAtUtc' é no futuro.</exception>
    /// <exception cref="ObjectDisposedException">Quando o serviço foi descartado.</exception>
    public async Task SendPasswordChangedAsync(
        EmailObject to,
        DateTime changedAtUtc,
        CancellationToken ct = default
    )
    {
        ValidateNotDisposed();
        _emailValidator.ValidateEmailAddress(to);
        _emailValidator.ValidateChangedTime(changedAtUtc);

        try
        {
            _loggingService.LogSendingEmail(EmailTypes.PasswordChanged, to.Value);

            var subject = EmailSubjects.PasswordChanged;
            var body = await _templateService.RenderPasswordChangedTemplateAsync(changedAtUtc);

            await _emailSender.SendAsync(to, subject, body, ct);

            _loggingService.LogEmailSentSuccessfully(EmailTypes.PasswordChanged, to.Value);
        }
        catch (Exception ex)
        {
            _loggingService.LogEmailSendFailed(EmailTypes.PasswordChanged, to.Value, ex);
            throw;
        }
    }

    /// <summary>
    /// Valida se o serviço não foi descartado.
    /// </summary>
    private void ValidateNotDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AccountEmailService));
    }

    /// <summary>
    /// Libera os recursos utilizados pelo serviço.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _emailSender?.Dispose();
            _disposed = true;
        }
    }
}
