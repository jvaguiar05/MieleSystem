using System.Security.Claims;

namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Métodos de extensão para facilitar o acesso a informações de Claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        if (user is null)
            return null;

        var id =
            user.FindFirstValueSafe(ClaimTypes.NameIdentifier) ?? user.FindFirstValueSafe("sub"); // fallback para JWT padrão

        return Guid.TryParse(id, out var guid) ? guid : null;
    }

    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user?.FindFirstValueSafe(ClaimTypes.Email) ?? user?.FindFirstValueSafe("email");
    }

    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user?.Identity?.IsAuthenticated ?? false;
    }

    public static bool IsInRole(this ClaimsPrincipal user, string role)
    {
        return user?.IsInRole(role) ?? false;
    }

    public static string? FindFirstValueSafe(this ClaimsPrincipal user, string claimType)
    {
        return user?.FindFirst(claimType)?.Value;
    }
}
