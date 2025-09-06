namespace MieleSystem.Domain.Identity.Services;

/// <summary>
/// Define um serviço para criar e verificar hashes de refresh tokens.
/// </summary>
public interface IRefreshTokenHasher
{
    /// <summary>
    /// Gera um hash a partir de um refresh token em texto plano.
    /// </summary>
    string Hash(string plainTextToken);
}
