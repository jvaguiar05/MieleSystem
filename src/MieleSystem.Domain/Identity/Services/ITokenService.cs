using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Domain.Identity.Services;

/// <summary>
/// Serviço de domínio responsável pela geração de tokens de autenticação.
/// Inclui tokens de acesso (JWT) e tokens de atualização (refresh).
/// A implementação concreta será fornecida pela camada Infrastructure.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um JWT válido para o usuário autenticado.
    /// </summary>
    /// <param name="user">Usuário autenticado.</param>
    /// <returns>Token JWT como string.</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Gera um token de atualização (refresh token).
    /// </summary>
    /// <returns>Token seguro e aleatório para renovação da sessão.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Obtém a data e hora de expiração do token de acesso.
    /// </summary>
    /// <returns>Data e hora de expiração do token de acesso.</returns>
    DateTime GetAccessTokenExpiration();

    /// <summary>
    /// Obtém a data e hora de expiração do refresh token.
    /// </summary>
    /// <returns>Data e hora de expiração do refresh token.</returns>
    DateTime GetRefreshTokenExpiration();
}
