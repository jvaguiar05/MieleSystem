using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Entities;

/// <summary>
/// Representa uma sessão OTP vinculada a um usuário.
/// É válida enquanto não estiver expirada nem usada.
/// </summary>
public class OtpSession : Entity
{
    public OtpCode Otp { get; private set; } = null!;

    /// <summary>Momento (UTC) em que a sessão foi criada.</summary>
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    /// <summary>Indica se o OTP desta sessão já foi utilizado.</summary>
    public bool IsUsed { get; private set; }

    /// <summary>Indica o propósito da sessão OTP.</summary>
    public OtpPurpose Purpose { get; private set; }

    /// <summary>Momento (UTC) em que a sessão foi utilizada.</summary>
    public DateTime? UsedAtUtc { get; private set; }

    // FK + navegação (backing para EF; permanece encapsulado no agregado)
    private int UserId { get; set; }
    private User User { get; set; } = null!;

    // Ctor para EF
    private OtpSession()
        : base(Guid.Empty) { }

    // Ctor restrito ao domínio (internal) — criação deve acontecer via User.AddOtpSession
    internal OtpSession(User user, OtpCode otp, OtpPurpose purpose)
        : base(Guid.Empty)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(otp);

        User = user;
        UserId = user.Id;

        Otp = otp;
        Purpose = purpose;
        CreatedAtUtc = DateTime.UtcNow;
        IsUsed = false;
    }

    /// <summary>
    /// Sessão está ativa enquanto não foi usada e o código não expirou.
    /// </summary>
    public bool IsActive => !IsUsed && !Otp.IsExpired();

    // Add this method to your OtpSession class
    public bool IsValid(string code)
    {
        // Use Otp.Matches for code comparison and Otp.IsExpired for expiration check
        return !IsUsed && Otp.Matches(code) && !Otp.IsExpired();
    }

    /// <summary>
    /// Verifica se o código informado corresponde ao OTP desta sessão
    /// e se a sessão ainda está ativa.
    /// </summary>
    public bool CanBeSatisfiedBy(string input) => IsActive && Otp.Matches(input);

    /// <summary>
    /// Marca esta sessão como utilizada. Idempotente.
    /// (A verificação de "é a sessão mais recente ativa do usuário"
    /// deve ser aplicada na camada de aplicação usando um ReadStore.)
    /// </summary>
    public void MarkAsUsed()
    {
        if (IsUsed)
            return;

        IsUsed = true;
        UsedAtUtc = DateTime.UtcNow;
        // Opcional: disparar um DomainEvent se você quiser auditar “OTP usado”.
        // AddDomainEvent(new OtpUsedEvent(User.PublicId, CreatedAtUtc));
    }

    /// <summary>
    /// Atalho seguro: valida o input e marca como usada, retornando sucesso/fracasso.
    /// Não lança exceções para fluxos de validação comuns.
    /// </summary>
    public bool TryValidateAndUse(string input)
    {
        if (!CanBeSatisfiedBy(input))
            return false;
        MarkAsUsed();
        return true;
    }

    /// <summary>
    /// Verifica se o propósito da sessão OTP é para login.
    /// </summary>
    /// <returns>Booleano indicando se o propósito é para login.</returns>
    public bool IsForLogin() => Purpose == OtpPurpose.Login;

    /// <summary>
    /// Verifica se o propósito da sessão OTP é para recuperação de senha.
    /// </summary>
    /// <returns>Booleano indicando se o propósito é para recuperação de senha.</returns>
    public bool IsForPasswordRecovery() => Purpose == OtpPurpose.PasswordRecovery;

    /// <summary>
    /// Verifica se o propósito da sessão OTP é para alteração de senha.
    /// </summary>
    /// <returns>Booleano indicando se o propósito é para alteração de senha.</returns>
    public bool IsForPasswordChange() => Purpose == OtpPurpose.PasswordChange;
}
