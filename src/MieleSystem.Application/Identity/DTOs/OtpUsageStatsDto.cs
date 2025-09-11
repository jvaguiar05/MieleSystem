using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// DTO para estatísticas de uso de OTP para fins administrativos e de monitoramento.
/// </summary>
public sealed record OtpUsageStatsDto
{
    /// <summary>Total de sessões OTP criadas no período.</summary>
    public int TotalSessionsCreated { get; init; }

    /// <summary>Total de sessões OTP utilizadas com sucesso no período.</summary>
    public int TotalSessionsUsed { get; init; }

    /// <summary>Total de sessões OTP expiradas no período.</summary>
    public int TotalSessionsExpired { get; init; }

    /// <summary>Total de sessões OTP ainda ativas (não utilizadas nem expiradas).</summary>
    public int TotalSessionsActive { get; init; }

    /// <summary>Taxa de sucesso de uso de OTP (utilizado / criado).</summary>
    public double SuccessRate { get; init; }

    /// <summary>Estatísticas por propósito de OTP.</summary>
    public Dictionary<OtpPurpose, OtpPurposeStatsDto> StatsByPurpose { get; init; } = new();

    /// <summary>Usuários únicos que geraram OTP no período.</summary>
    public int UniqueUsersWithOtp { get; init; }

    /// <summary>Período de análise (em dias).</summary>
    public int PeriodInDays { get; init; }

    /// <summary>Data de início da análise.</summary>
    public DateTime StartDate { get; init; }

    /// <summary>Data de fim da análise.</summary>
    public DateTime EndDate { get; init; }
}

/// <summary>
/// DTO para estatísticas de OTP por propósito específico.
/// </summary>
public sealed record OtpPurposeStatsDto
{
    /// <summary>Propósito do OTP.</summary>
    public OtpPurpose Purpose { get; init; }

    /// <summary>Total de sessões criadas para este propósito.</summary>
    public int TotalCreated { get; init; }

    /// <summary>Total de sessões utilizadas para este propósito.</summary>
    public int TotalUsed { get; init; }

    /// <summary>Total de sessões expiradas para este propósito.</summary>
    public int TotalExpired { get; init; }

    /// <summary>Taxa de sucesso para este propósito.</summary>
    public double SuccessRate { get; init; }
}
