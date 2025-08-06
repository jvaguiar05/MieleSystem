using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Entities;

/// <summary>
/// Representa um token de atualização (refresh token) associado a um usuário.
/// Usado para manter sessões autenticadas com renovação de acesso.
/// </summary>
public class RefreshToken : Entity
{
    public Token Token { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public bool IsRevoked { get; private set; } = false;

    public DateTime? RevokedAt { get; private set; }

    // FK oculta (privada)
    // Usado para mapear o relacionamento com o usuário no ORM (EF Core)
    private int UserId { get; set; }

    // Navegação (privada, se preferir manter agregação isolada)
    private User User { get; set; } = null!;

    /// Construtor para uso do ORM (EF Core)
    private RefreshToken()
        : base(Guid.Empty) { }

    /// Cria um novo refresh token com expiração e PublicId gerado automaticamente.
    public RefreshToken(Token token, DateTime expiresAt)
        : base(Guid.Empty)
    {
        Token = token;
        ExpiresAt = expiresAt;
    }

    /// Cria um refresh token com PublicId explícito (para testes ou reidratação).
    public RefreshToken(Guid publicId, Token token, DateTime expiresAt)
        : base(publicId)
    {
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public void Revoke()
    {
        if (IsRevoked)
            return;

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;

    public bool IsInvalid() => IsRevoked || IsExpired();
}
