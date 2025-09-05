using System.Security.Claims;
using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Extensões para o ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtém o ID do usuário.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        if (user is null)
            return null;

        // Ordem padrão em JWT é "sub"; NameIdentifier cobre cenários ASP.NET Identity
        var id =
            user.FindFirstValue("sub")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("uid")
            ?? user.FindFirstValue("userId")
            ?? user.FindFirstValue("publicId");

        return Guid.TryParse(id, out var guid) ? guid : null;
    }

    /// <summary>
    /// Obtém o email do usuário.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static string? GetEmail(this ClaimsPrincipal user)
    {
        if (user is null)
            return null;

        // Email "canônico" + fallbacks comuns de IdP
        return user.FindFirstValue(ClaimTypes.Email)
            ?? user.FindFirstValue("email")
            ?? user.FindFirstValue("preferred_username");
    }

    /// <summary>
    /// Verifica se o usuário está autenticado.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static bool IsAuthenticated(this ClaimsPrincipal user) =>
        user?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Verifica se o usuário está em um determinado role.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public static bool IsInRole(this ClaimsPrincipal user, string role) =>
        user?.GetRole() == role;

    /// <summary>
    /// Obtém o role do usuário atual.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static string? GetRole(this ClaimsPrincipal user)
    {
        return user?.FindFirst(ClaimTypes.Role)?.Value;
    }
}
