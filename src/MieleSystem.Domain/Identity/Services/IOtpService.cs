using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Services;

/// <summary>
/// Serviço de domínio responsável pela geração e verificação de códigos OTP (One-Time Password).
/// A implementação concreta (ex: algoritmo TOTP, random, etc.) será fornecida na camada Infrastructure.
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Gera um novo código OTP com base em parâmetros internos do sistema.
    /// </summary>
    /// <returns>Objeto de valor representando o código OTP.</returns>
    OtpCode Generate();

    /// <summary>
    /// Verifica se o código fornecido é válido com base no esperado.
    /// </summary>
    /// <param name="expected">Código originalmente gerado pelo sistema.</param>
    /// <param name="provided">Código fornecido pelo usuário.</param>
    /// <returns>True se os códigos corresponderem; false caso contrário.</returns>
    bool Validate(OtpCode expected, string provided);
}
