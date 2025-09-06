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
/// <param name="db">Contexto do banco de dados</param>
/// <param name="mapper">Instância do AutoMapper</param>
public sealed class UserReadStore(MieleDbContext db, IMapper mapper) : IUserReadStore
{
    private readonly MieleDbContext _db = db;
    private readonly IMapper _mapper = mapper;

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
