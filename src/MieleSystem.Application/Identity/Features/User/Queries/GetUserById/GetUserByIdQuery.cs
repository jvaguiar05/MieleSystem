using MediatR;
using MieleSystem.Application.Identity.DTOs;

namespace MieleSystem.Application.Identity.Features.User.Queries.GetUserById;

/// <summary>
/// Query para buscar um usuário pelo seu ID público.
/// </summary>
public sealed class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid PublicId { get; init; }
}
