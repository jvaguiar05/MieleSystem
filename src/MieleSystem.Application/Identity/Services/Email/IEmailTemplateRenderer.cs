namespace MieleSystem.Application.Identity.Services.Email;

/// <summary>
/// Responsável por renderizar o conteúdo (body) de e-mails enviados no contexto de Identity.
/// Essa interface permite customizar ou trocar a estratégia de formatação facilmente.
/// </summary>
public interface IEmailTemplateRenderer
{
    string RenderWelcome(string userName);
    string RenderOtp(string code, DateTime expiresAtUtc);
    string RenderPasswordChanged(DateTime changedAtUtc);
    string RenderRegistrationApproved(string userName);
}
