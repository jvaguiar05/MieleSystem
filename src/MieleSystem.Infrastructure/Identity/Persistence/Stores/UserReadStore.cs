using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MieleSystem.Application.Common.DTOs;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Infrastructure.Common.Persistence;

namespace MieleSystem.Infrastructure.Identity.Persistence.Stores;

/// <summary>
/// Armazena dados de leitura de usuários.
/// Utilizado para consultas e operações de leitura.
/// </summary>
/// <param name="db"></param>
/// <param name="mapper"></param>
public sealed class UserReadStore(MieleDbContext db, IMapper mapper) : IUserReadStore
{
    private readonly MieleDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Obtém uma lista paginada de usuários.
    /// Serve para exibir usuários em uma interface de usuário.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns>Uma lista paginada de usuários. Se não houver usuários, a lista será vazia.</returns>
    public async Task<PageResultDto<UserListItemDto>> GetPagedAsync(
        PageRequestDto request,
        CancellationToken ct = default
    )
    {
        var query = _db
            .Users.AsNoTracking()
            .OrderByDescending(u => u.CreatedAtUtc)
            .ProjectTo<UserListItemDto>(_mapper.ConfigurationProvider);

        var total = await query.CountAsync(ct);

        var items = await query.Skip(request.Skip).Take(request.PageSize).ToListAsync(ct);

        return PageResultDto<UserListItemDto>.Create(
            items: items,
            page: request.Page,
            pageSize: request.PageSize,
            totalCount: total
        );
    }
}
