namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Contrato para envio de e-mails assíncronos.
/// A responsabilidade de implementação pertence à camada de infraestrutura.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envia um e-mail com assunto e corpo para o destinatário informado.
    /// </summary>
    /// <param name="to">Endereço de e-mail do destinatário.</param>
    /// <param name="subject">Assunto do e-mail.</param>
    /// <param name="body">Corpo do e-mail (em HTML ou texto).</param>
    /// <param name="isHtml">Indica se o corpo do e-mail está em HTML.</param>
    /// <param name="cancellationToken">Token de cancelamento opcional.</param>
    Task SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default
    );
}
