namespace MieleSystem.Domain.Identity.Services;

/// <summary>
/// Serviço de domínio responsável por gerar e validar hashes de senha.
/// A implementação concreta (ex: BCrypt, Argon2) é fornecida na camada Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gera um hash seguro a partir de uma senha em texto puro.
    /// </summary>
    /// <param name="plainTextPassword">Senha em texto puro.</param>
    /// <returns>Hash seguro da senha.</returns>
    string Hash(string plainTextPassword);

    /// <summary>
    /// Verifica se a senha em texto puro corresponde ao hash fornecido.
    /// </summary>
    /// <param name="hashedPassword">Hash armazenado.</param>
    /// <param name="plainTextPassword">Senha fornecida pelo usuário.</param>
    /// <returns>True se a senha for válida; caso contrário, false.</returns>
    bool Verify(string hashedPassword, string plainTextPassword);

    /// <summary>
    /// Verifica se o hash da senha precisa ser refeito.
    /// </summary>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    bool NeedsRehash(string hashedPassword);
}
