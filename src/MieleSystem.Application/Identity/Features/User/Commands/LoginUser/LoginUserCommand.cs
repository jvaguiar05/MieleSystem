using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.DTOs;

namespace MieleSystem.Application.Identity.Features.User.Commands.LoginUser;

/// <summary>
/// DTO interno para transportar os resultados do login do Handler para o Controller.
/// </summary>
public sealed record LoginUserResult(
    LoginResultDto Dto,
    string PlainTextRefreshToken,
    DateTime RefreshTokenExpiresAtUtc
);

/// <summary>
/// Comando para autenticação de usuário via e-mail e senha.
/// Agora retorna um LoginUserResult, contendo o DTO e o refresh token separadamente.
/// </summary>
public sealed class LoginUserCommand : IRequest<Result<LoginUserResult>>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? ClientIp { get; init; }
    public string? UserAgent { get; init; }
    public string? DeviceId { get; init; }
}
