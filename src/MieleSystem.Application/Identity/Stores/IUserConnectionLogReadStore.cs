using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Application.Identity.Stores;

/// <summary>
/// Store para leitura de dados de conexão de usuários.
/// </summary>
public interface IUserConnectionLogReadStore
{
    /// <summary>
    /// Obtém os logs de conexão de um usuário por período.
    /// </summary>
    Task<IEnumerable<UserConnectionLog>> GetByUserIdAsync(
        int userId,
        int days = 30,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém logs de conexão por endereço IP.
    /// </summary>
    Task<IEnumerable<UserConnectionLog>> GetByIpAddressAsync(
        string ipAddress,
        int days = 7,
        CancellationToken ct = default
    );

    /// <summary>
    /// Verifica se um IP é conhecido para um usuário.
    /// </summary>
    Task<bool> IsKnownIpForUserAsync(
        int userId,
        string ipAddress,
        int days = 30,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém estatísticas de conexão de um usuário.
    /// </summary>
    Task<ConnectionStatsDto> GetConnectionStatsAsync(
        int userId,
        int days = 30,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém logs suspeitos para análise de segurança.
    /// </summary>
    Task<IEnumerable<UserConnectionLog>> GetSuspiciousConnectionsAsync(
        int days = 7,
        CancellationToken ct = default
    );
}
