using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Infrastructure.Common.Persistence;

namespace MieleSystem.Infrastructure.Identity.Persistence.Stores;

/// <summary>
/// Implementação do store para leitura de dados de sessões OTP.
/// Utiliza AutoMapper para converter entidades em DTOs.
/// </summary>
public sealed class OtpSessionReadStore(MieleDbContext context, IMapper mapper)
    : IOtpSessionReadStore
{
    private readonly MieleDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    private IQueryable<OtpSession> GetActiveSessionsQuery()
    {
        var now = DateTime.UtcNow;
        return _context
            .Set<OtpSession>()
            .Where(session => !session.IsUsed && session.Otp.ExpiresAt > now)
            .AsNoTracking();
    }

    public async Task<OtpSessionDto?> GetLatestActiveSessionAsync(
        int userId,
        OtpPurpose purpose,
        CancellationToken ct = default
    )
    {
        return await GetActiveSessionsQuery()
            .Where(session => EF.Property<int>(session, "UserId") == userId)
            .Where(session => session.Purpose == purpose)
            .OrderByDescending(session => session.CreatedAtUtc)
            .ProjectTo<OtpSessionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<OtpSessionDto>> GetActiveSessionsByUserIdAsync(
        int userId,
        CancellationToken ct = default
    )
    {
        return await GetActiveSessionsQuery()
            .Where(session => EF.Property<int>(session, "UserId") == userId)
            .OrderByDescending(session => session.CreatedAtUtc)
            .ProjectTo<OtpSessionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<OtpSessionDto?> GetActiveSessionByCodeAsync(
        int userId,
        string code,
        OtpPurpose purpose,
        CancellationToken ct = default
    )
    {
        return await GetActiveSessionsQuery()
            .Where(session => EF.Property<int>(session, "UserId") == userId)
            .Where(session => session.Otp.Code == code)
            .Where(session => session.Purpose == purpose)
            .OrderByDescending(session => session.CreatedAtUtc)
            .ProjectTo<OtpSessionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<OtpSessionDto>> GetSessionsByUserAndPurposeAsync(
        int userId,
        OtpPurpose purpose,
        int days = 7,
        CancellationToken ct = default
    )
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        return await _context
            .Set<OtpSession>()
            .Where(session => EF.Property<int>(session, "UserId") == userId)
            .Where(session => session.Purpose == purpose)
            .Where(session => session.CreatedAtUtc >= cutoffDate)
            .OrderByDescending(session => session.CreatedAtUtc)
            .AsNoTracking()
            .ProjectTo<OtpSessionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<OtpSessionDto>> GetAllSessionsForAdminAsync(
        int? userId = null,
        OtpPurpose? purpose = null,
        int days = 30,
        CancellationToken ct = default
    )
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        var query = _context
            .Set<OtpSession>()
            .Where(session => session.CreatedAtUtc >= cutoffDate)
            .AsNoTracking();

        if (userId.HasValue)
            query = query.Where(session => EF.Property<int>(session, "UserId") == userId.Value);

        if (purpose.HasValue)
            query = query.Where(session => session.Purpose == purpose.Value);

        return await query
            .OrderByDescending(session => session.CreatedAtUtc)
            .ProjectTo<OtpSessionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<OtpUsageStatsDto> GetUsageStatsAsync(
        int days = 7,
        CancellationToken ct = default
    )
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        var endDate = DateTime.UtcNow;
        var now = DateTime.UtcNow;

        var allSessions = await _context
            .Set<OtpSession>()
            .Where(session => session.CreatedAtUtc >= startDate)
            .AsNoTracking()
            .ToListAsync(ct);

        var totalCreated = allSessions.Count;
        var totalUsed = allSessions.Count(s => s.IsUsed);
        var totalExpired = allSessions.Count(s => !s.IsUsed && s.Otp.ExpiresAt <= now);
        var totalActive = allSessions.Count(s => !s.IsUsed && s.Otp.ExpiresAt > now);

        var successRate = totalCreated > 0 ? (double)totalUsed / totalCreated : 0;

        var statsByPurpose = allSessions
            .GroupBy(s => s.Purpose)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var created = g.Count();
                    var used = g.Count(s => s.IsUsed);
                    var expired = g.Count(s => !s.IsUsed && s.Otp.ExpiresAt <= now);
                    var rate = created > 0 ? (double)used / created : 0;

                    return new OtpPurposeStatsDto
                    {
                        Purpose = g.Key,
                        TotalCreated = created,
                        TotalUsed = used,
                        TotalExpired = expired,
                        SuccessRate = rate,
                    };
                }
            );

        var uniqueUsers = allSessions.Select(s => EF.Property<int>(s, "UserId")).Distinct().Count();

        return new OtpUsageStatsDto
        {
            TotalSessionsCreated = totalCreated,
            TotalSessionsUsed = totalUsed,
            TotalSessionsExpired = totalExpired,
            TotalSessionsActive = totalActive,
            SuccessRate = successRate,
            StatsByPurpose = statsByPurpose,
            UniqueUsersWithOtp = uniqueUsers,
            PeriodInDays = days,
            StartDate = startDate,
            EndDate = endDate,
        };
    }
}
