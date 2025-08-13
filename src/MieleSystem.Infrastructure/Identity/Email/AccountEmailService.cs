using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using MieleSystem.Application.Identity.Services;
using MieleSystem.Infrastructure.Identity.Options;
using EmailObject = MieleSystem.Domain.Identity.ValueObjects.Email;

namespace MieleSystem.Infrastructure.Identity.Email;

public sealed class AccountEmailService : IAccountEmailService
{
    private readonly EmailSenderOptions _options;
    private readonly SmtpClient _smtp;
    private readonly SimpleEmailTemplateRenderer _templates = new();

    public AccountEmailService(IOptions<EmailSenderOptions> options)
    {
        _options = options.Value;

        _smtp = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
        {
            Credentials = new NetworkCredential(_options.FromEmail, _options.SmtpPassword),
            EnableSsl = _options.UseSsl,
        };
    }

    public async Task SendWelcomeAsync(
        EmailObject to,
        string userName,
        CancellationToken ct = default
    )
    {
        var subject = "Bem-vindo ao MieleSystem!";
        var body = _templates.RenderWelcome(userName);
        await SendAsync(to, subject, body, ct);
    }

    public async Task SendOtpAsync(
        EmailObject to,
        string code,
        DateTime expiresAtUtc,
        CancellationToken ct = default
    )
    {
        var subject = "Código de verificação - MieleSystem";
        var body = _templates.RenderOtp(code, expiresAtUtc);
        await SendAsync(to, subject, body, ct);
    }

    public async Task SendPasswordChangedAsync(
        EmailObject to,
        DateTime changedAtUtc,
        CancellationToken ct = default
    )
    {
        var subject = "Sua senha foi alterada";
        var body = _templates.RenderPasswordChanged(changedAtUtc);
        await SendAsync(to, subject, body, ct);
    }

    private async Task SendAsync(
        EmailObject to,
        string subject,
        string htmlBody,
        CancellationToken ct
    )
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };

        message.To.Add(to.Value);

        await _smtp.SendMailAsync(message, ct);
    }
}
