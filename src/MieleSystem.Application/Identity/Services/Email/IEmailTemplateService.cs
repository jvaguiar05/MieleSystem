namespace MieleSystem.Application.Identity.Services.Email;

/// <summary>
/// Interface para renderização de templates de e-mail.
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Renderiza o template de boas-vindas.
    /// </summary>
    /// <param name="userName">Nome do usuário.</param>
    /// <returns>Conteúdo HTML do template.</returns>
    /// <exception cref="InvalidOperationException">Quando a renderização falha.</exception>
    Task<string> RenderWelcomeTemplateAsync(string userName);

    /// <summary>
    /// Renderiza o template de código OTP.
    /// </summary>
    /// <param name="code">Código OTP.</param>
    /// <param name="expiresAtUtc">Data de expiração.</param>
    /// <returns>Conteúdo HTML do template.</returns>
    /// <exception cref="InvalidOperationException">Quando a renderização falha.</exception>
    Task<string> RenderOtpTemplateAsync(string code, DateTime expiresAtUtc);

    /// <summary>
    /// Renderiza o template de alteração de senha.
    /// </summary>
    /// <param name="changedAtUtc">Data da alteração.</param>
    /// <returns>Conteúdo HTML do template.</returns>
    /// <exception cref="InvalidOperationException">Quando a renderização falha.</exception>
    Task<string> RenderPasswordChangedTemplateAsync(DateTime changedAtUtc);

    /// <summary>
    /// Renderiza o template de aprovação de registro.
    /// </summary>
    /// <param name="userName">Nome do usuário.</param>
    /// <returns>Conteúdo HTML do template.</returns>
    /// <exception cref="InvalidOperationException">Quando a renderização falha.</exception>
    Task<string> RenderRegistrationApprovedTemplateAsync(string userName);
}
