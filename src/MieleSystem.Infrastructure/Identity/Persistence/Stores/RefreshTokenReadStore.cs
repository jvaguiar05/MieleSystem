using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.ValueObjects;
using MieleSystem.Infrastructure.Common.Persistence;

namespace MieleSystem.Infrastructure.Identity.Persistence.Stores;

/// <summary>
/// Armazena dados de leitura de refresh tokens.
/// Utilizado apenas para consultas e operações de leitura.
/// </summary>
public sealed class RefreshTokenReadStore(MieleDbContext db, IMapper mapper)
    : IRefreshTokenReadStore
{
    private readonly MieleDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<RefreshTokenDto?> GetByTokenHashAsync(
        RefreshTokenHash tokenHash,
        CancellationToken cancellationToken = default
    )
    {
        return await _db
            .RefreshTokens.AsNoTracking()
            .Where(rt => rt.TokenHash.Value == tokenHash.Value)
            .ProjectTo<RefreshTokenDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetActiveTokensByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _db
            .RefreshTokens.AsNoTracking()
            .Where(rt =>
                EF.Property<int>(rt, "UserId") == userId
                && !rt.IsRevoked
                && rt.ExpiresAtUtc > DateTime.UtcNow
            )
            .ProjectTo<RefreshTokenDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
