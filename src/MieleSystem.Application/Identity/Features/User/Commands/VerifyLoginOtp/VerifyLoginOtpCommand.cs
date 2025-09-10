using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.DTOs;

namespace MieleSystem.Application.Identity.Features.User.Commands.VerifyLoginOtp;

/// <summary>
/// DTO interno para transportar os resultados da verificação OTP do Handler para o Controller.
/// </summary>
public sealed record VerifyLoginOtpResult(
    LoginResultDto Dto,
    string PlainTextRefreshToken,
    DateTime RefreshTokenExpiresAtUtc
);

/// <summary>
/// Comando para verificar OTP durante o processo de login.
/// </summary>
public sealed class VerifyLoginOtpCommand : IRequest<Result<VerifyLoginOtpResult>>
{
    public string Email { get; init; } = null!;
    public string OtpCode { get; init; } = null!;
    public string? ClientIp { get; init; }
    public string? UserAgent { get; init; }
    public string? DeviceId { get; init; }
}
