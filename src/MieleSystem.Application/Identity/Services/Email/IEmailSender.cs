using Mail = MieleSystem.Domain.Identity.ValueObjects.Email;

namespace MieleSystem.Application.Identity.Services.Email;

/// <summary>
/// Interface para envio de e-mails.
/// </summary>
public interface IEmailSender : IDisposable
{
    /// <summary>
    /// Envia um e-mail usando o provedor configurado.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino.</param>
    /// <param name="subject">Assunto do e-mail.</param>
    /// <param name="htmlBody">Corpo HTML do e-mail.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <exception cref="ObjectDisposedException">Quando o serviço foi descartado.</exception>
    Task SendAsync(Mail to, string subject, string htmlBody, CancellationToken ct = default);
}
