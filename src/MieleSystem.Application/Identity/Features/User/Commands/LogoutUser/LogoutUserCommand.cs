using System.Text.Json.Serialization;
using MediatR;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Identity.Features.User.Commands.LogoutUser;

/// <summary>
/// Comando para logout do usuário.
/// Revoga o refresh token ativo e registra log de conexão.
/// </summary>
public sealed class LogoutUserCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Refresh token que será revogado (obtido do cookie HttpOnly).
    /// </summary>
    [JsonIgnore]
    public string? RefreshToken { get; init; }

    /// <summary>
    /// ID público do usuário autenticado (extraído do token JWT).
    /// Usado como fallback se o refresh token não for válido.
    /// </summary>
    [JsonIgnore]
    public Guid? CurrentUserPublicId { get; init; }

    [JsonIgnore]
    public string? ClientIp { get; init; }

    [JsonIgnore]
    public string? UserAgent { get; init; }

    [JsonIgnore]
    public string? DeviceId { get; init; }
}
