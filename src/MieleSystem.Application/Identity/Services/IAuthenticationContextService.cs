using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Application.Identity.Services;

/// <summary>
/// Service to determine authentication context and OTP requirements based on various factors.
/// </summary>
public interface IAuthenticationContextService
{
    /// <summary>
    /// Determines if OTP verification is required for the given user and login context.
    /// </summary>
    /// <param name="user">The user attempting to login.</param>
    /// <param name="clientInfo">Optional client information for risk assessment.</param>
    /// <returns>True if OTP is required, false otherwise.</returns>
    Task<bool> IsOtpRequiredAsync(User user, AuthenticationClientInfo? clientInfo = null);
}

/// <summary>
/// Contains client information for authentication context evaluation.
/// </summary>
public record AuthenticationClientInfo(string? IpAddress, string? UserAgent, string? DeviceId);
