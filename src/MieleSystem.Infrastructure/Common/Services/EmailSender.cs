using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Infrastructure.Common.Services;

/// <summary>
/// Implementação padrão de IEmailSender usando SMTP.
/// Configurações devem ser fornecidas via appsettings.json ou variáveis de ambiente.
/// </summary>
internal sealed class EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    : IEmailSender
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<EmailSender> _logger = logger;

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken ct = default
    )
    {
        var smtpHost = _configuration["Email:Smtp:Host"];
        var smtpPort = int.TryParse(_configuration["Email:Smtp:Port"], out var port) ? port : 587;
        var smtpUser = _configuration["Email:Smtp:Username"];
        var smtpPass = _configuration["Email:Smtp:Password"];
        var fromEmail = _configuration["Email:From"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true,
        };

        using var message = new MailMessage
        {
            From = new MailAddress(
                fromEmail
                    ?? smtpUser
                    ?? throw new InvalidOperationException("Remetente não configurado.")
            ),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml,
        };

        message.To.Add(to);

        try
        {
            _logger.LogInformation(
                "Enviando e-mail para {To} com assunto '{Subject}'",
                to,
                subject
            );
            await client.SendMailAsync(message, ct);
            _logger.LogInformation("E-mail enviado com sucesso para {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail para {To}", to);
            throw;
        }
    }
}
