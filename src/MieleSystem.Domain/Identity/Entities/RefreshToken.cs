using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Exceptions;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Entities;

/// <summary>
/// Token de atualização pertencente ao agregado <see cref="User"/>.
/// Ciclo de vida controlado exclusivamente pelo Aggregate Root.
/// </summary>
public sealed class RefreshToken : Entity
{
    public Token Token { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    // FK + navegação (privadas, mantêm o encapsulamento do agregado)
    private int UserId { get; set; }
    private User User { get; set; } = null!;

    // EF Core
    private RefreshToken()
        : base(Guid.Empty) { }

    /// <summary>
    /// Construtor restrito para o agregado. Garante vínculo imediato ao <see cref="User"/>.
    /// </summary>
    /// <param name="owner">Usuário dono do token (Aggregate Root).</param>
    /// <param name="token">Valor opaco do refresh token.</param>
    /// <param name="expiresAtUtc">Expiração em UTC (deve ser futura).</param>
    /// <param name="publicId">Opcional para reidratação/teste. Se vazio, é gerado.</param>
    internal RefreshToken(User owner, Token token, DateTime expiresAtUtc, Guid? publicId = null)
        : base(publicId ?? Guid.Empty)
    {
        if (owner is null)
            throw new DomainException("Usuário do refresh token é obrigatório.");
        if (token is null)
            throw new DomainException("Token inválido.");
        if (expiresAtUtc <= DateTime.UtcNow)
            throw new DomainException("Expiração do refresh token deve ser futura.");

        User = owner;
        UserId = owner.Id;

        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        IsRevoked = false;
        RevokedAtUtc = null;
    }

    /// <summary>Indica se o token está expirado (UTC).</summary>
    public bool IsExpired() => DateTime.UtcNow >= ExpiresAtUtc;

    /// <summary>Ativo quando não revogado e não expirado.</summary>
    public bool IsActive() => !IsRevoked && !IsExpired();

    /// <summary>Revoga o token (idempotente).</summary>
    public void Revoke()
    {
        if (IsRevoked)
            return;

        IsRevoked = true;
        RevokedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Inválido quando expirado ou revogado.</summary>
    public bool IsInvalid() => IsRevoked || IsExpired();

    /// <summary>
    /// (Opcional) Extensão controlada do tempo de expiração — útil para rotinas de “rotate/extend”.
    /// Mantém a regra de que a nova data deve ser futura e maior que a atual.
    /// </summary>
    public void ExtendExpiration(DateTime newExpiresAtUtc)
    {
        if (IsRevoked)
            throw new DomainException("Não é possível estender um token revogado.");
        if (newExpiresAtUtc <= DateTime.UtcNow)
            throw new DomainException("Nova expiração deve ser futura.");
        if (newExpiresAtUtc <= ExpiresAtUtc)
            throw new DomainException("Nova expiração deve ser maior que a atual.");

        ExpiresAtUtc = newExpiresAtUtc;
    }
}
