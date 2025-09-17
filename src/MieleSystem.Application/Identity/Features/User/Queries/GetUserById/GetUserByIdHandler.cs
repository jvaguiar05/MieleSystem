using MediatR;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Stores;

namespace MieleSystem.Application.Identity.Features.User.Queries.GetUserById;

/// <summary>
/// Handler responsável por buscar um usuário pelo seu ID público.
/// </summary>
public sealed class GetUserByIdHandler(IUserReadStore userReadStore)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserReadStore _userReadStore = userReadStore;

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user =
            await _userReadStore.GetByPublicIdAsync(request.PublicId, cancellationToken)
            ?? throw new ApplicationException(
                "Usuário de ID " + request.PublicId + " não foi encontrado."
            );

        return user;
    }
}
