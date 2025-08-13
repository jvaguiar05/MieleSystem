using MediatR;
using MieleSystem.Application.Common.DTOs;
using MieleSystem.Application.Identity.DTOs;

namespace MieleSystem.Application.Identity.Features.User.Queries.ListUsersPaged;

/// <summary>
/// Query para buscar usuários com paginação.
/// </summary>
public sealed class ListUsersPagedQuery
    : PageRequestDto,
        IRequest<PageResultDto<UserListItemDto>> { }
