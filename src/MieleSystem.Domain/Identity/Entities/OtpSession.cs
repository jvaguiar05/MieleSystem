using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Entities;

/// <summary>
/// Representa uma sessão de verificação OTP associada a um usuário.
/// Utilizada para validar tentativas e controlar expiração.
/// </summary>
public class OtpSession : Entity
{
    public OtpCode Otp { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public bool IsUsed { get; private set; }

    public DateTime? UsedAt { get; private set; }

    // FK oculta (privada)
    private int UserId { get; set; }

    // Navegação (privada, para manter agregação isolada)
    private User User { get; set; } = null!;

    /// Construtor para ORM
    private OtpSession()
        : base(Guid.Empty) { }

    public OtpSession(User user, OtpCode otp)
        : base(Guid.Empty)
    {
        Otp = otp;
        CreatedAt = DateTime.UtcNow;
        IsUsed = false;

        User = user;
        UserId = user.Id; // opcional, EF mapeia automático com navigation. Manter para clareza
    }

    /// Verifica se o OTP pode ser usado.
    public bool IsValid(string input) => !IsUsed && !Otp.IsExpired() && Otp.Matches(input);

    public void MarkAsUsed()
    {
        if (IsUsed)
            return;

        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }
}
