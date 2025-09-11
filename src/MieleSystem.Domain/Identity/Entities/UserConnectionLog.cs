using MieleSystem.Domain.Common.Base;

namespace MieleSystem.Domain.Identity.Entities;

/// <summary>
/// Representa um log de conexão/tentativa de login de um usuário.
/// Usado para análise de padrões, detecção de atividades suspeitas e contexto de autenticação.
/// </summary>
public sealed class UserConnectionLog : Entity
{
    public string IpAddress { get; private set; } = null!;
    public string UserAgent { get; private set; } = null!;
    public string? DeviceId { get; private set; }
    public string? Location { get; private set; }
    public DateTime ConnectedAtUtc { get; private set; } = DateTime.UtcNow;
    public bool IsSuccessful { get; private set; }
    public bool RequiredOtp { get; private set; }
    public string? OtpReason { get; private set; }
    public string? AdditionalInfo { get; private set; }

    // FK para User
    public int UserId { get; set; }
    private User User { get; set; } = null!;

    // EF Core
    private UserConnectionLog()
        : base(Guid.Empty) { }

    /// <summary>
    /// Construtor interno para ser chamado apenas pelo agregado User.
    /// </summary>
    internal UserConnectionLog(
        User user,
        string ipAddress,
        string userAgent,
        string? deviceId = null,
        string? location = null,
        string? additionalInfo = null
    )
        : base(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(userAgent);

        User = user;
        UserId = user.Id;
        IpAddress = ipAddress.Trim();
        UserAgent = userAgent.Trim();
        DeviceId = deviceId?.Trim();
        Location = location?.Trim();
        AdditionalInfo = additionalInfo?.Trim();
        ConnectedAtUtc = DateTime.UtcNow;
        IsSuccessful = false; // Será atualizado quando necessário
        RequiredOtp = false;
    }

    /// <summary>
    /// Marca esta conexão como bem-sucedida.
    /// </summary>
    public void MarkAsSuccessful()
    {
        IsSuccessful = true;
    }

    /// <summary>
    /// Marca que esta conexão requereu OTP.
    /// </summary>
    /// <param name="reason">Motivo pelo qual OTP foi necessário.</param>
    public void MarkOtpRequired(string reason)
    {
        RequiredOtp = true;
        OtpReason = reason?.Trim();
    }

    /// <summary>
    /// Verifica se esta conexão é de um IP conhecido (últimos 30 dias).
    /// </summary>
    public bool IsFromKnownIp(IEnumerable<UserConnectionLog> recentConnections)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        return recentConnections
            .Where(log => log.ConnectedAtUtc >= thirtyDaysAgo && log.IsSuccessful)
            .Any(log => log.IpAddress == IpAddress);
    }

    /// <summary>
    /// Verifica se esta é uma conexão suspeita baseada em padrões.
    /// </summary>
    public bool IsSuspicious(IEnumerable<UserConnectionLog> recentConnections)
    {
        var last24Hours = DateTime.UtcNow.AddHours(-24);
        var recentLogs = recentConnections.Where(log => log.ConnectedAtUtc >= last24Hours).ToList();

        // Muitas tentativas nas últimas 24h
        if (recentLogs.Count >= 10)
            return true;

        // IP nunca usado antes
        if (!IsFromKnownIp(recentConnections))
            return true;

        // Múltiplos IPs diferentes nas últimas 24h
        var distinctIps = recentLogs.Select(log => log.IpAddress).Distinct().Count();
        if (distinctIps >= 3)
            return true;

        return false;
    }
}
