using Microsoft.EntityFrameworkCore;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Infrastructure.Common.Persistence;

namespace MieleSystem.Infrastructure.Identity.Persistence.Stores;

/// <summary>
/// Implementação do store para leitura de dados de conexão de usuários.
/// </summary>
public sealed class UserConnectionLogReadStore(MieleDbContext context) : IUserConnectionLogReadStore
{
    private readonly MieleDbContext _context =
        context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IEnumerable<UserConnectionLog>> GetByUserIdAsync(
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
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<UserConnectionLog>> GetByIpAddressAsync(
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

        var logs = await _context
            .Set<UserConnectionLog>()
            .Where(log => EF.Property<int>(log, "UserId") == userId)
            .Where(log => log.ConnectedAtUtc >= cutoffDate)
            .AsNoTracking()
            .ToListAsync(ct);

        if (!logs.Any())
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

        var totalConnections = logs.Count;
        var successfulConnections = logs.Count(l => l.IsSuccessful);
        var failedConnections = totalConnections - successfulConnections;
        var uniqueIps = logs.Select(l => l.IpAddress).Distinct().Count();
        var lastSuccessfulConnection = logs.Where(l => l.IsSuccessful)
            .OrderByDescending(l => l.ConnectedAtUtc)
            .FirstOrDefault()
            ?.ConnectedAtUtc;
        var lastConnection = logs.Max(l => l.ConnectedAtUtc);

        return new ConnectionStatsDto
        {
            SuccessfulConnections = successfulConnections,
            FailedConnections = failedConnections,
            UniqueIpAddresses = uniqueIps,
            LastSuccessfulConnection = lastSuccessfulConnection,
            LastConnectionAttempt = lastConnection,
        };
    }

    public async Task<IEnumerable<UserConnectionLog>> GetSuspiciousConnectionsAsync(
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
            .ToListAsync(ct);
    }
}
