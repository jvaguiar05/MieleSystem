using Microsoft.Extensions.Logging;
using MieleSystem.Application.Identity.Services.Email;

namespace MieleSystem.Infrastructure.Identity.Email;

/// <summary>
/// Serviço responsável pela renderização de templates de e-mail.
/// </summary>
public sealed class EmailTemplateService : IEmailTemplateService
{
    private static readonly Action<ILogger, string, Exception?> TemplateRenderingFailed =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1007, nameof(TemplateRenderingFailed)),
            "Falha ao renderizar template: {TemplateType}"
        );

    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly ILogger<EmailTemplateService> _logger;

    /// <summary>
    /// Inicializa uma nova instância do serviço de templates.
    /// </summary>
    /// <param name="templateRenderer">Renderizador de templates.</param>
    /// <param name="logger">Logger para registro de eventos.</param>
    public EmailTemplateService(
        IEmailTemplateRenderer templateRenderer,
        ILogger<EmailTemplateService> logger
    )
    {
        _templateRenderer =
            templateRenderer ?? throw new ArgumentNullException(nameof(templateRenderer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Renderiza um template de e-mail de forma assíncrona com tratamento de exceções.
    /// </summary>
    /// <param name="renderFunction">Função que renderiza o template.</param>
    /// <param name="templateType">Tipo do template para logging.</param>
    /// <returns>Conteúdo HTML renderizado.</returns>
    /// <exception cref="InvalidOperationException">Quando a renderização falha.</exception>
    public async Task<string> RenderTemplateAsync(Func<string> renderFunction, string templateType)
    {
        try
        {
            return await Task.Run(renderFunction);
        }
        catch (Exception ex)
        {
            TemplateRenderingFailed(_logger, templateType, ex);
            throw new InvalidOperationException($"Falha ao renderizar template {templateType}", ex);
        }
    }

    /// <summary>
    /// Renderiza o template de boas-vindas.
    /// </summary>
    /// <param name="userName">Nome do usuário.</param>
    /// <returns>Conteúdo HTML do template.</returns>
    public async Task<string> RenderWelcomeTemplateAsync(string userName)
    {
        return await RenderTemplateAsync(
            () => _templateRenderer.RenderWelcome(userName),
            "Welcome"
        );
    }

    /// <summary>
    /// Renderiza o template de código OTP.
    /// </summary>
    /// <param name="code">Código OTP.</param>
    /// <param name="expiresAtUtc">Data de expiração.</param>
    /// <returns>Conteúdo HTML do template.</returns>
    public async Task<string> RenderOtpTemplateAsync(string code, DateTime expiresAtUtc)
    {
        return await RenderTemplateAsync(
            () => _templateRenderer.RenderOtp(code, expiresAtUtc),
            "OTP"
        );
    }

    /// <summary>
    /// Renderiza o template de alteração de senha.
    /// </summary>
    /// <param name="changedAtUtc">Data da alteração.</param>
    /// <returns>Conteúdo HTML do template.</returns>
    public async Task<string> RenderPasswordChangedTemplateAsync(DateTime changedAtUtc)
    {
        return await RenderTemplateAsync(
            () => _templateRenderer.RenderPasswordChanged(changedAtUtc),
            "PasswordChanged"
        );
    }
}
