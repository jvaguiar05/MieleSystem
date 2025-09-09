using MieleSystem.Application.Identity.Services;

namespace MieleSystem.Presentation.Middlewares;

/// <summary>
/// Middleware responsável por capturar e armazenar informações de contexto da requisição
/// para uso posterior no processo de autenticação.
/// </summary>
public sealed class AuthenticationContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext context)
    {
        // Captura informações da requisição
        var clientInfo = ExtractClientInfo(context);

        // Armazena no HttpContext para uso posterior
        context.Items["AuthenticationClientInfo"] = clientInfo;

        await _next(context);
    }

    private static AuthenticationClientInfo ExtractClientInfo(HttpContext context)
    {
        var request = context.Request;

        // Extrai IP Address
        var ipAddress = GetClientIpAddress(context);

        // Extrai User-Agent
        var userAgent = request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(userAgent))
            userAgent = "Unknown";

        // Extrai Device ID (pode vir de um header customizado ou cookie)
        var deviceId =
            request.Headers["X-Device-Id"].FirstOrDefault()
            ?? request.Cookies["device-id"]
            ?? GenerateDeviceId(context);

        return new AuthenticationClientInfo(ipAddress, userAgent, deviceId);
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Tenta obter o IP real considerando proxies e load balancers
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(ipAddress))
        {
            // X-Forwarded-For pode conter múltiplos IPs separados por vírgula
            ipAddress = ipAddress.Split(',')[0].Trim();
        }

        // Fallback para outros headers comuns
        ipAddress ??= context.Request.Headers["X-Real-IP"].FirstOrDefault();
        ipAddress ??= context.Request.Headers["CF-Connecting-IP"].FirstOrDefault(); // Cloudflare
        ipAddress ??= context.Connection.RemoteIpAddress?.ToString();

        // Fallback final
        return string.IsNullOrWhiteSpace(ipAddress) ? "Unknown" : ipAddress;
    }

    private static string GenerateDeviceId(HttpContext context)
    {
        // Gera um ID de dispositivo baseado em características da requisição
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();
        var acceptEncoding = context.Request.Headers.AcceptEncoding.ToString();

        var fingerprint = $"{userAgent}|{acceptLanguage}|{acceptEncoding}";

        // Cria um hash simples para usar como device ID
        var hashBytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(fingerprint)
        );
        var deviceId = Convert.ToHexString(hashBytes)[..16]; // Primeiros 16 caracteres

        // Define um cookie para futuras requisições
        context.Response.Cookies.Append(
            "device-id",
            deviceId,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
            }
        );

        return deviceId;
    }
}

/// <summary>
/// Extensões para facilitar o registro do middleware.
/// </summary>
public static class AuthenticationContextMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de contexto de autenticação ao pipeline.
    /// </summary>
    public static IApplicationBuilder UseAuthenticationContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationContextMiddleware>();
    }
}
