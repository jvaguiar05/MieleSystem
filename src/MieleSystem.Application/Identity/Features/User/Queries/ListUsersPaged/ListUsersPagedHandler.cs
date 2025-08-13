using AutoMapper;
using MediatR;
using MieleSystem.Application.Common.DTOs;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Stores;

namespace MieleSystem.Application.Identity.Features.User.Queries.ListUsersPaged;

public sealed class ListUsersPagedHandler(IUserReadStore store)
    : IRequestHandler<ListUsersPagedQuery, PageResultDto<UserListItemDto>>
{
    private readonly IUserReadStore _store = store;

    public async Task<PageResultDto<UserListItemDto>> Handle(
        ListUsersPagedQuery request,
        CancellationToken ct
    )
    {
        return await _store.GetPagedAsync(request, ct);
    }
}
