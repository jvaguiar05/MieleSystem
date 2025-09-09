using Microsoft.AspNetCore.Http;
using MieleSystem.Application.Identity.Services;

namespace MieleSystem.Infrastructure.Identity.Services;

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

/// <summary>
/// Implementação do serviço que extrai contexto de autenticação do HttpContext.
/// </summary>
public sealed class HttpContextAuthenticationService(IHttpContextAccessor httpContextAccessor)
    : IHttpContextAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

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
