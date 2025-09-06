using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Stores;

/// <summary>
/// Fonte de leitura otimizada para consultas de RefreshToken.
/// Retorna DTOs para operações de consulta e verificação.
/// </summary>
public interface IRefreshTokenReadStore
{
    /// <summary>
    /// Busca um refresh token pelo hash.
    /// </summary>
    /// <param name="tokenHash">Hash do token a ser buscado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>RefreshTokenDto encontrado ou null se não existir.</returns>
    Task<RefreshTokenDto?> GetByTokenHashAsync(
        RefreshTokenHash tokenHash,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Busca refresh tokens ativos de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de RefreshTokenDto ativos.</returns>
    Task<List<RefreshTokenDto>> GetActiveTokensByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
}
