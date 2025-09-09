using Microsoft.Extensions.Logging;
using MieleSystem.Application.Identity.Services;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Infrastructure.Identity.Services;

/// <summary>
/// Implementação do serviço para determinar o contexto de autenticação e requisitos de OTP.
/// </summary>
public sealed class AuthenticationContextService(
    ILogger<AuthenticationContextService> logger,
    IUserConnectionLogReadStore connectionLogStore
) : IAuthenticationContextService
{
    private readonly ILogger<AuthenticationContextService> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUserConnectionLogReadStore _connectionLogStore =
        connectionLogStore ?? throw new ArgumentNullException(nameof(connectionLogStore));

    public async Task<bool> IsOtpRequiredAsync(
        User user,
        AuthenticationClientInfo? clientInfo = null
    )
    {
        // Lógica para determinar se OTP é necessário
        var requiresOtp = await EvaluateOtpRequirement(user, clientInfo);

        if (requiresOtp)
            _logger.LogInformation(
                "OTP required for user {UserId} due to security policy",
                user.PublicId
            );

        return requiresOtp;
    }

    private async Task<bool> EvaluateOtpRequirement(User user, AuthenticationClientInfo? clientInfo)
    {
        if (clientInfo == null || string.IsNullOrWhiteSpace(clientInfo.IpAddress))
            return user.Role?.Name == "Admin";

        // 1. Sempre requer OTP para usuários Admin
        if (user.Role?.Name == "Admin")
        {
            _logger.LogInformation("OTP required for Admin user {UserId}", user.PublicId);
            return true;
        }

        // 2. Verifica se o IP é desconhecido (nunca usado com sucesso nos últimos 30 dias)
        var isKnownIp = await _connectionLogStore.IsKnownIpForUserAsync(
            user.Id,
            clientInfo.IpAddress,
            30
        );
        if (!isKnownIp)
        {
            _logger.LogInformation(
                "OTP required for unknown IP {IpAddress} for user {UserId}",
                clientInfo.IpAddress,
                user.PublicId
            );
            return true;
        }

        // 3. Verifica padrões de atividade suspeita
        var connectionStats = await _connectionLogStore.GetConnectionStatsAsync(user.Id, 1); // Últimas 24h
        if (connectionStats.FailedConnections >= 3)
        {
            _logger.LogInformation(
                "OTP required due to recent failed attempts for user {UserId}",
                user.PublicId
            );
            return true;
        }

        // 4. Verifica se o usuário tem um padrão de atividade suspeita
        if (user.HasSuspiciousActivity(clientInfo.IpAddress))
        {
            _logger.LogInformation(
                "OTP required due to suspicious activity for user {UserId}",
                user.PublicId
            );
            return true;
        }

        // 5. Políticas baseadas em tempo (fora do horário comercial = mais rigoroso)
        if (!IsBusinessHours())
        {
            // 20% de chance de OTP durante o horário não comercial
            var random = new Random();
            if (random.Next(100) < 20)
            {
                _logger.LogInformation(
                    "OTP required due to non-business hours policy for user {UserId}",
                    user.PublicId
                );
                return true;
            }
        }

        // 6. Verificação de segurança aleatória (5% de logins durante o horário comercial)
        if (IsBusinessHours())
        {
            var random = new Random();
            if (random.Next(100) < 5)
            {
                _logger.LogInformation(
                    "OTP required due to random security check for user {UserId}",
                    user.PublicId
                );
                return true;
            }
        }

        // Default: não requer OTP
        return false;
    }

    private static bool IsBusinessHours()
    {
        var now = DateTime.Now;
        return now.Hour >= 9
            && now.Hour <= 17
            && now.DayOfWeek != DayOfWeek.Saturday
            && now.DayOfWeek != DayOfWeek.Sunday;
    }
}
