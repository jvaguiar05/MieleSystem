using Microsoft.Extensions.Logging;
using MieleSystem.Application.Identity.Services.Email;
using EmailObject = MieleSystem.Domain.Identity.ValueObjects.Email;

namespace MieleSystem.Infrastructure.Identity.Services.Email;

/// <summary>
/// Orquestrador responsável pelo envio de e-mails relacionados a operações de conta.
/// Coordena validação, renderização de templates, logging e envio de e-mails.
/// </summary>
/// <remarks>
/// Inicializa uma nova instância do orquestrador de e-mails.
/// </remarks>
public sealed class AccountEmailService(
    IEmailSender emailSender,
    IEmailTemplateService templateService,
    IEmailLoggingService loggingService,
    IEmailValidator emailValidator,
    ILogger<AccountEmailService> logger
) : IAccountEmailService, IDisposable
{
    private static class EmailTypes
    {
        public const string Welcome = "Welcome";
        public const string Otp = "OTP";
        public const string PasswordChanged = "PasswordChanged";
        public const string RegistrationApproved = "RegistrationApproved";
    }

    private static class EmailSubjects
    {
        public const string Welcome = "Bem-vindo ao MieleSystem!";
        public const string Otp = "Código de verificação - MieleSystem";
        public const string PasswordChanged = "Sua senha foi alterada";
        public const string RegistrationApproved = "Sua conta foi aprovada - MieleSystem";
    }

    private readonly IEmailSender _emailSender = emailSender;
    private readonly IEmailTemplateService _templateService = templateService;
    private readonly IEmailLoggingService _loggingService = loggingService;
    private readonly IEmailValidator _emailValidator = emailValidator;
    private bool _disposed;

    /// <summary>
    /// Envia um e-mail de boas-vindas para um novo usuário.
    /// Desabilitado temporariamente.
    /// </summary>
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

            // await _emailSender.SendAsync(to, subject, body, ct);

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

            // await _emailSender.SendAsync(to, subject, body, ct);

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

            // await _emailSender.SendAsync(to, subject, body, ct);

            _loggingService.LogEmailSentSuccessfully(EmailTypes.PasswordChanged, to.Value);
        }
        catch (Exception ex)
        {
            _loggingService.LogEmailSendFailed(EmailTypes.PasswordChanged, to.Value, ex);
            throw;
        }
    }

    /// <summary>
    /// Envia um e-mail notificando que o registro foi aprovado.
    /// </summary>
    public async Task SendRegistrationApprovedAsync(
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
            _loggingService.LogSendingEmail(EmailTypes.RegistrationApproved, to.Value);

            var subject = EmailSubjects.RegistrationApproved;
            var body = await _templateService.RenderRegistrationApprovedTemplateAsync(userName);

            // await _emailSender.SendAsync(to, subject, body, ct);

            _loggingService.LogEmailSentSuccessfully(EmailTypes.RegistrationApproved, to.Value);
        }
        catch (Exception ex)
        {
            _loggingService.LogEmailSendFailed(EmailTypes.RegistrationApproved, to.Value, ex);
            throw;
        }
    }

    /// <summary>
    /// Valida se o serviço não foi descartado.
    /// </summary>
    private void ValidateNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(AccountEmailService));
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
