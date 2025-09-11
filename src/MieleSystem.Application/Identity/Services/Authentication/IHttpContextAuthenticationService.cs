using MieleSystem.Application.Identity.Services.Authentication;

namespace MieleSystem.Application.Identity.Services.Authentication;

/// <summary>
/// Serviço para extrair informações de contexto de autenticação a partir do HttpContext.
/// </summary>
public interface IHttpContextAuthenticationService
{
    /// <summary>
    /// Extrai as informações de cliente da requisição atual.
    /// </summary>
    AuthenticationClientInfo? GetCurrentClientInfo();
}
