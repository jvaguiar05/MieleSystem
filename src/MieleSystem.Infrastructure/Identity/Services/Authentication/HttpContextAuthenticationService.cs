using Microsoft.AspNetCore.Http;
using MieleSystem.Application.Identity.Services.Authentication;

namespace MieleSystem.Infrastructure.Identity.Services.Authentication;

/// <summary>
/// Implementação do serviço que extrai contexto de autenticação do HttpContext.
/// </summary>
public sealed class HttpContextAuthenticationService(IHttpContextAccessor httpContextAccessor)
    : IHttpContextAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public AuthenticationClientInfo? GetCurrentClientInfo()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        // Tenta obter as informações que foram capturadas pelo middleware
        if (
            httpContext.Items.TryGetValue("AuthenticationClientInfo", out var clientInfoObj)
            && clientInfoObj is AuthenticationClientInfo clientInfo
        )
            return clientInfo;

        // Fallback: extrai diretamente do HttpContext se o middleware não estiver configurado
        return ExtractClientInfoDirectly(httpContext);
    }

    /// <summary>
    /// Extrai as informações de cliente diretamente do HttpContext.
    /// Utilizado como fallback caso o middleware não esteja configurado.
    /// </summary>
    /// <param name="context">Contexto HTTP.</param>
    /// <returns>Informações do cliente extraídas.</returns>
    private static AuthenticationClientInfo ExtractClientInfoDirectly(HttpContext context)
    {
        var request = context.Request;

        // Extrai IP Address
        var ipAddress = GetClientIpAddress(context);

        // Extrai User-Agent
        var userAgent = request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(userAgent))
            userAgent = "Unknown";

        // Extrai Device ID
        var deviceId =
            request.Headers["X-Device-Id"].FirstOrDefault() ?? request.Cookies["device-id"];

        return new AuthenticationClientInfo(ipAddress, userAgent, deviceId);
    }

    /// <summary>
    /// Obtém o endereço IP do cliente a partir do HttpContext.
    /// Considera cabeçalhos comuns de proxy.
    /// Caso de uso: Extração de informações de cliente para autenticação.
    /// </summary>
    /// <param name="context">Contexto HTTP.</param>
    /// <returns>Endereço IP do cliente.</returns>
    private static string GetClientIpAddress(HttpContext context)
    {
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(ipAddress))
            ipAddress = ipAddress.Split(',')[0].Trim();

        ipAddress ??= context.Request.Headers["X-Real-IP"].FirstOrDefault();
        ipAddress ??= context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        ipAddress ??= context.Connection.RemoteIpAddress?.ToString();

        return string.IsNullOrWhiteSpace(ipAddress) ? "Unknown" : ipAddress;
    }
}
