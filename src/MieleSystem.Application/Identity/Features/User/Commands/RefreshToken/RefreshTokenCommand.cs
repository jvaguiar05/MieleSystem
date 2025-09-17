using System.Text.Json.Serialization;
using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.Features.User.Commands.LoginUser;

namespace MieleSystem.Application.Identity.Features.User.Commands.RefreshToken;

/// <summary>
/// Comando para renovação de token de acesso usando refresh token.
/// Retorna um LoginUserResult, contendo o DTO e o novo refresh token separadamente.
/// </summary>
public sealed class RefreshTokenCommand : IRequest<Result<LoginUserResult>>
{
    /// <summary>
    /// Refresh token fornecido (geralmente vem de um cookie HttpOnly).
    /// </summary>
    public string RefreshToken { get; init; } = null!;

    [JsonIgnore]
    public string? ClientIp { get; init; }

    [JsonIgnore]
    public string? UserAgent { get; init; }

    [JsonIgnore]
    public string? DeviceId { get; init; }
}
