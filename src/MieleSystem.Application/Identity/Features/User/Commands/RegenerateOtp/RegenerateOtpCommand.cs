using MediatR;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Identity.Features.User.Commands.RegenerateOtp;

/// <summary>
/// Comando para regenerar um código OTP expirado.
/// Permite ao usuário solicitar um novo código quando o anterior expirou.
/// </summary>
public sealed record RegenerateOtpCommand : IRequest<Result<RegenerateOtpResult>>
{
    public string Email { get; init; } = null!;
    public string? ClientIp { get; init; }
    public string? UserAgent { get; init; }
    public string? DeviceId { get; init; }
}

/// <summary>
/// Resultado do comando de regeneração de OTP.
/// </summary>
public sealed record RegenerateOtpResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = null!;
    public DateTime? NewExpirationTime { get; init; }
    public int AttemptsRemaining { get; init; }
}
