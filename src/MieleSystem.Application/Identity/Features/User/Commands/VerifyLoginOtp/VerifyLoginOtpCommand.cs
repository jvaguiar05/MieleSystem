using System.Text.Json.Serialization;
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

    [JsonIgnore]
    public string? ClientIp { get; init; }

    [JsonIgnore]
    public string? UserAgent { get; init; }

    [JsonIgnore]
    public string? DeviceId { get; init; }
}
