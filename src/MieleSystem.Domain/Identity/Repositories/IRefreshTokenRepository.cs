using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Repositories;

/// <summary>
/// Repositório para gerenciamento de tokens de atualização (refresh tokens).
/// Permite buscar, verificar e manipular tokens diretamente.
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    // Busca um refresh token pelo valor do token.
    Task<RefreshToken?> GetByTokenAsync(Token token, CancellationToken cancellationToken = default);

    // Busca todos os tokens válidos de um usuário.
    Task<IReadOnlyList<RefreshToken>> GetValidTokensByUserIdAsync(
        Guid publicUserId,
        CancellationToken cancellationToken = default
    );

    // Remove todos os tokens expirados ou revogados (limpeza periódica, se necessário).
    Task RemoveInvalidTokensAsync(CancellationToken cancellationToken = default);
}
