using System.Security.Cryptography;
using System.Text;
using MieleSystem.Domain.Identity.Services;

namespace MieleSystem.Infrastructure.Identity.Services.Domain;

/// <summary>
/// Implementação do hasher de refresh tokens utilizando o algoritmo SHA-256.
/// </summary>
public sealed class RefreshTokenHasher : IRefreshTokenHasher
{
    public string Hash(string plainTextToken)
    {
        // Garante que a entrada não seja nula ou vazia.
        if (string.IsNullOrEmpty(plainTextToken))
            throw new ArgumentNullException(
                nameof(plainTextToken),
                "Token não pode ser nulo ou vazio."
            );

        // Converte o token de string para um array de bytes
        var tokenBytes = Encoding.UTF8.GetBytes(plainTextToken);

        // Calcula o hash
        var hashBytes = SHA256.HashData(tokenBytes);

        // Converte o array de bytes do hash para uma string hexadecimal
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
