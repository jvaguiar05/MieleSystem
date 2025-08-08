using System.Security.Claims;

namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Extensões para o ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
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

    public static string? GetEmail(this ClaimsPrincipal user)
    {
        if (user is null)
            return null;

        // Email "canônico" + fallbacks comuns de IdP
        return user.FindFirstValue(ClaimTypes.Email)
            ?? user.FindFirstValue("email")
            ?? user.FindFirstValue("preferred_username");
    }

    public static bool IsAuthenticated(this ClaimsPrincipal user) =>
        user?.Identity?.IsAuthenticated ?? false;

    public static bool IsInRole(this ClaimsPrincipal user, string role) =>
        user?.IsInRole(role) ?? false;
}
