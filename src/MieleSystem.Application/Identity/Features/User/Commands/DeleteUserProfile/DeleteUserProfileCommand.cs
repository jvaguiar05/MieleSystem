using System.Text.Json.Serialization;
using MediatR;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Identity.Features.User.Commands.DeleteUserProfile;

/// <summary>
/// Comando para um usuário deletar seu próprio perfil (soft delete).
/// O usuário é identificado através do token de autenticação.
/// </summary>
public sealed class DeleteUserProfileCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// ID público do usuário autenticado (extraído do token JWT).
    /// </summary>
    [JsonIgnore]
    public Guid CurrentUserPublicId { get; init; }
}
