using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Application.Identity.Stores;

/// <summary>
/// Store para leitura de dados de sessões OTP.
/// Retorna DTOs para operações de consulta e verificação.
/// </summary>
public interface IOtpSessionReadStore
{
    /// <summary>
    /// Obtém a sessão OTP mais recente ativa de um usuário para um propósito específico.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="purpose">Propósito do OTP.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>DTO da sessão OTP ativa ou nulo se não existir.</returns>
    Task<OtpSessionDto?> GetLatestActiveSessionAsync(
        int userId,
        OtpPurpose purpose,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém todas as sessões OTP ativas de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de DTOs das sessões OTP ativas.</returns>
    Task<IEnumerable<OtpSessionDto>> GetActiveSessionsByUserIdAsync(
        int userId,
        CancellationToken ct = default
    );

    /// <summary>
    /// Verifica se existe uma sessão OTP ativa para o código fornecido.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="code">Código OTP a ser verificado.</param>
    /// <param name="purpose">Propósito do OTP.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>DTO da sessão OTP se encontrada e ativa; nulo caso contrário.</returns>
    Task<OtpSessionDto?> GetActiveSessionByCodeAsync(
        int userId,
        string code,
        OtpPurpose purpose,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém sessões OTP por usuário e propósito.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="purpose">Propósito do OTP.</param>
    /// <param name="days">Número de dias para buscar sessões (padrão é 7).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de DTOs das sessões OTP encontradas.</returns>
    Task<IEnumerable<OtpSessionDto>> GetSessionsByUserAndPurposeAsync(
        int userId,
        OtpPurpose purpose,
        int days = 7,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém todas as sessões OTP (ativas, expiradas e usadas) para fins administrativos.
    /// </summary>
    /// <param name="userId">ID do usuário (opcional para buscar todas).</param>
    /// <param name="purpose">Propósito do OTP (opcional).</param>
    /// <param name="days">Número de dias para buscar sessões (padrão é 30).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de DTOs das sessões OTP encontradas.</returns>
    Task<IEnumerable<OtpSessionDto>> GetAllSessionsForAdminAsync(
        int? userId = null,
        OtpPurpose? purpose = null,
        int days = 30,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém estatísticas de uso de OTP para monitoramento.
    /// </summary>
    /// <param name="days">Número de dias para calcular estatísticas (padrão é 7).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Estatísticas de uso de OTP.</returns>
    Task<OtpUsageStatsDto> GetUsageStatsAsync(int days = 7, CancellationToken ct = default);
}
