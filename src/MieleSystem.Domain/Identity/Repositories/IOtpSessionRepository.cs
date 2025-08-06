using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Domain.Identity.Repositories;

/// <summary>
/// Repositório para acesso e manipulação das sessões OTP de um usuário.
/// Útil para validação de códigos e auditoria.
/// </summary>
public interface IOtpSessionRepository : IRepository<OtpSession>
{
    // Retorna a sessão OTP ativa (não utilizada e não expirada) mais recente de um usuário
    Task<OtpSession?> GetActiveSessionByUserIdAsync(
        Guid publicUserId,
        CancellationToken cancellationToken = default
    );

    // Retorna todas as sessões OTP de um usuário
    Task<IReadOnlyList<OtpSession>> GetAllByUserIdAsync(
        Guid publicUserId,
        CancellationToken cancellationToken = default
    );
}
