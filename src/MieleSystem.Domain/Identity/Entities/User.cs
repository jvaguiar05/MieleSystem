using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Exceptions;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Events.Admin;
using MieleSystem.Domain.Identity.Events.User;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Entities;

public sealed class User : AggregateRoot, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    /// Situação do registro do usuário.
    public UserRegistrationSituation RegistrationSituation { get; private set; } =
        UserRegistrationSituation.Pending;

    /// <summary>Data de expiração da conta, se aplicável.</summary>
    public DateOnly? ExpiresAt { get; private set; }

    // -----------------------------
    // Child entities (backing fields)
    // -----------------------------
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private readonly List<OtpSession> _otpSessions = new();
    public IReadOnlyCollection<OtpSession> OtpSessions => _otpSessions.AsReadOnly();

    // -----------------------------
    // Soft delete
    // -----------------------------
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // -----------------------------
    // Ctors / factories
    // -----------------------------
    // EF
    private User()
        : base(Guid.Empty) { }

    private User(Guid publicId, string name, Email email, PasswordHash passwordHash, UserRole role)
        : base(publicId)
    {
        ChangeName(name); // valida internamente
        Email = email ?? throw new DomainException("E-mail inválido.");
        PasswordHash = NewPasswordHashOrThrow(passwordHash);
        Role = role ?? throw new DomainException("Role inválida.");
    }

    public static User Register(
        string name,
        Email email,
        PasswordHash passwordHash,
        UserRole role
    ) => new(Guid.NewGuid(), name, email, passwordHash, role);

    // -----------------------------
    // Domain methods (guards first)
    // -----------------------------
    private void EnsureNotDeleted()
    {
        if (IsDeleted)
            throw new DomainException("Operação não permitida: usuário deletado.");
    }

    private static PasswordHash NewPasswordHashOrThrow(PasswordHash hash) =>
        hash ?? throw new DomainException("Hash de senha inválido.");

    public void MarkAsRegistered()
    {
        EnsureNotDeleted();

        AddDomainEvent(new UserRegisteredEvent(Id, PublicId, Role, Email, Name));
    }

    public void ChangeName(string newName)
    {
        EnsureNotDeleted();

        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Nome inválido.");

        if (newName.Trim() == Name)
            return;

        Name = newName.Trim();
        AddDomainEvent(new UserUpdatedEvent(PublicId));
    }

    public void ChangePassword(PasswordHash newPasswordHash)
    {
        EnsureNotDeleted();

        PasswordHash = NewPasswordHashOrThrow(newPasswordHash);
        AddDomainEvent(new PasswordChangedEvent(PublicId));
    }

    public void PromoteTo(UserRole role)
    {
        EnsureNotDeleted();

        if (role is null)
            throw new DomainException("Role inválida.");
        if (role.Equals(Role))
            return;

        Role = role;
        AddDomainEvent(new UserRoleChangedEvent(PublicId, role));
    }

    /// <summary>Define a conta como temporária, com expiração programada.</summary>
    public void MarkAsTemporary(DateOnly expiresAt)
    {
        EnsureNotDeleted();

        if (expiresAt <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Data de expiração deve ser futura.");

        ExpiresAt = expiresAt;
    }

    // -----------------------------
    // Refresh Tokens lifecycle (owned by aggregate)
    // -----------------------------
    public RefreshToken AddRefreshToken(Token token, DateTime expiresAtUtc)
    {
        EnsureNotDeleted();

        if (token is null)
            throw new DomainException("Token inválido.");
        if (expiresAtUtc <= DateTime.UtcNow)
            throw new DomainException("Expiração do refresh token deve ser futura.");

        var refreshToken = new RefreshToken(this, token, expiresAtUtc, null);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public bool RevokeRefreshToken(Token token)
    {
        EnsureNotDeleted();

        var rt = _refreshTokens.FirstOrDefault(x => x.Token == token);
        if (rt is null)
            return false;

        rt.Revoke();
        return true;
    }

    /// <summary>Revoga todos os tokens ativos (ex.: logout global, segurança).</summary>
    public int RevokeAllRefreshTokens()
    {
        EnsureNotDeleted();

        var count = 0;
        foreach (var rt in _refreshTokens.Where(x => !x.IsInvalid()))
        {
            rt.Revoke();
            count++;
        }
        return count;
    }

    /// <summary>Remove tokens inválidos do agregado (expirados/revogados).</summary>
    public int RemoveInvalidRefreshTokens()
    {
        EnsureNotDeleted();

        var before = _refreshTokens.Count;
        _refreshTokens.RemoveAll(rt => rt.IsInvalid());
        return before - _refreshTokens.Count;
    }

    // -----------------------------
    // OTP lifecycle (owned by aggregate)
    // -----------------------------
    public OtpSession AddOtpSession(OtpCode otp, OtpPurpose purpose)
    {
        EnsureNotDeleted();

        if (otp is null)
            throw new DomainException("OTP inválido.");

        // estratégia simples: permitir múltiplas sessões; consumo controla validade
        var session = new OtpSession(this, otp, purpose);
        _otpSessions.Add(session);
        return session;
    }

    /// <summary>Tenta consumir a sessão OTP mais recente válida para o código informado.</summary>
    public bool TryConsumeOtp(string code)
    {
        EnsureNotDeleted();

        var session = _otpSessions
            .OrderByDescending(s => s.CreatedAtUtc)
            .FirstOrDefault(s => s.IsValid(code));

        if (session is null)
            return false;

        session.MarkAsUsed();
        return true;
    }

    // -----------------------------
    // Soft delete
    // -----------------------------
    public void Delete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;

        AddDomainEvent(new UserDeletedEvent(PublicId));
    }

    public void Restore()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAt = null;
    }

    // -----------------------------
    // Registration workflow
    // -----------------------------
    public void ApproveRegistration()
    {
        EnsureNotDeleted();

        if (RegistrationSituation != UserRegistrationSituation.Pending)
            throw new DomainException("Registro já processado.");

        RegistrationSituation = UserRegistrationSituation.Accepted;
        AddDomainEvent(new UserRegistrationApprovedEvent(PublicId));
    }

    public void DeclineRegistration(string reason)
    {
        EnsureNotDeleted();

        if (RegistrationSituation != UserRegistrationSituation.Pending)
            throw new DomainException("Registro já processado.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Motivo da rejeição é obrigatório.");

        RegistrationSituation = UserRegistrationSituation.Rejected;
        AddDomainEvent(new UserRegistrationRejectedEvent(PublicId, reason.Trim()));
    }
}
