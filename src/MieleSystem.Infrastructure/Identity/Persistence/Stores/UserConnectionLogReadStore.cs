using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Infrastructure.Common.Persistence;

namespace MieleSystem.Infrastructure.Identity.Persistence.Stores;

/// <summary>
/// Implementação do store para leitura de dados de conexão de usuários.
/// Utiliza AutoMapper para converter entidades em DTOs.
/// </summary>
public sealed class UserConnectionLogReadStore(MieleDbContext context, IMapper mapper)
    : IUserConnectionLogReadStore
{
    private readonly MieleDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<UserConnectionLogDto>> GetByUserIdAsync(
        int userId,
        int days = 30,
        CancellationToken ct = default
    )
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        return await _context
            .Set<UserConnectionLog>()
            .Where(log => EF.Property<int>(log, "UserId") == userId)
            .Where(log => log.ConnectedAtUtc >= cutoffDate)
            .OrderByDescending(log => log.ConnectedAtUtc)
            .AsNoTracking()
            .ProjectTo<UserConnectionLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<UserConnectionLogDto>> GetByIpAddressAsync(
        string ipAddress,
        int days = 7,
        CancellationToken ct = default
    )
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        return await _context
            .Set<UserConnectionLog>()
            .Where(log => log.IpAddress == ipAddress)
            .Where(log => log.ConnectedAtUtc >= cutoffDate)
            .OrderByDescending(log => log.ConnectedAtUtc)
            .AsNoTracking()
            .ProjectTo<UserConnectionLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<bool> IsKnownIpForUserAsync(
        int userId,
        string ipAddress,
        int days = 30,
        CancellationToken ct = default
    )
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        return await _context
            .Set<UserConnectionLog>()
            .Where(log => EF.Property<int>(log, "UserId") == userId)
            .Where(log => log.IpAddress == ipAddress)
            .Where(log => log.ConnectedAtUtc >= cutoffDate)
            .Where(log => log.IsSuccessful)
            .AsNoTracking()
            .AnyAsync(ct);
    }

    public async Task<ConnectionStatsDto> GetConnectionStatsAsync(
        int userId,
        int days = 30,
        CancellationToken ct = default
    )
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        var logsQuery = _context
            .Set<UserConnectionLog>()
            .Where(log => EF.Property<int>(log, "UserId") == userId)
            .Where(log => log.ConnectedAtUtc >= cutoffDate)
            .AsNoTracking();

        var hasAnyLogs = await logsQuery.AnyAsync(ct);

        if (!hasAnyLogs)
        {
            return new ConnectionStatsDto
            {
                SuccessfulConnections = 0,
                FailedConnections = 0,
                UniqueIpAddresses = 0,
                LastSuccessfulConnection = null,
                LastConnectionAttempt = null,
            };
        }

        var successfulConnections = await logsQuery.CountAsync(l => l.IsSuccessful, ct);
        var totalConnections = await logsQuery.CountAsync(ct);
        var failedConnections = totalConnections - successfulConnections;
        var uniqueIps = await logsQuery.Select(l => l.IpAddress).Distinct().CountAsync(ct);
        var lastSuccessfulConnection = await logsQuery
            .Where(l => l.IsSuccessful)
            .OrderByDescending(l => l.ConnectedAtUtc)
            .Select(l => (DateTime?)l.ConnectedAtUtc)
            .FirstOrDefaultAsync(ct);
        var lastConnection = await logsQuery.MaxAsync(l => l.ConnectedAtUtc, ct);

        return new ConnectionStatsDto
        {
            SuccessfulConnections = successfulConnections,
            FailedConnections = failedConnections,
            UniqueIpAddresses = uniqueIps,
            LastSuccessfulConnection = lastSuccessfulConnection,
            LastConnectionAttempt = lastConnection,
        };
    }

    public async Task<IEnumerable<UserConnectionLogDto>> GetSuspiciousConnectionsAsync(
        int days = 7,
        CancellationToken ct = default
    )
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        // Conexões com múltiplas tentativas falhadas do mesmo IP
        var suspiciousIps = await _context
            .Set<UserConnectionLog>()
            .Where(log => log.ConnectedAtUtc >= cutoffDate)
            .Where(log => !log.IsSuccessful)
            .GroupBy(log => log.IpAddress)
            .Where(g => g.Count() >= 5) // 5 ou mais falhas
            .Select(g => g.Key)
            .ToListAsync(ct);

        return await _context
            .Set<UserConnectionLog>()
            .Where(log => log.ConnectedAtUtc >= cutoffDate)
            .Where(log => suspiciousIps.Contains(log.IpAddress))
            .OrderByDescending(log => log.ConnectedAtUtc)
            .AsNoTracking()
            .ProjectTo<UserConnectionLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
