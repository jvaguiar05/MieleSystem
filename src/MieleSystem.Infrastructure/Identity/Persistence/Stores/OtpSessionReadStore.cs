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

    public async Task<OtpSessionDto?> GetLatestActiveSessionAsync(
        int userId,
        OtpPurpose purpose,
        CancellationToken ct = default
    )
    {
        var now = DateTime.UtcNow;

        return await _context
            .Set<OtpSession>()
            .Where(session => EF.Property<int>(session, "UserId") == userId)
            .Where(session => session.Purpose == purpose)
            .Where(session => !session.IsUsed)
            .Where(session => session.Otp.ExpiresAt > now)
            .OrderByDescending(session => session.CreatedAtUtc)
            .AsNoTracking()
            .ProjectTo<OtpSessionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<OtpSessionDto>> GetActiveSessionsByUserIdAsync(
        int userId,
        CancellationToken ct = default
    )
    {
        var now = DateTime.UtcNow;

        return await _context
            .Set<OtpSession>()
            .Where(session => EF.Property<int>(session, "UserId") == userId)
            .Where(session => !session.IsUsed)
            .Where(session => session.Otp.ExpiresAt > now)
            .OrderByDescending(session => session.CreatedAtUtc)
            .AsNoTracking()
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
        var now = DateTime.UtcNow;

        return await _context
            .Set<OtpSession>()
            .Where(session => EF.Property<int>(session, "UserId") == userId)
            .Where(session => session.Purpose == purpose)
            .Where(session => !session.IsUsed)
            .Where(session => session.Otp.ExpiresAt > now)
            .Where(session => session.Otp.Code == code)
            .OrderByDescending(session => session.CreatedAtUtc)
            .AsNoTracking()
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
}
