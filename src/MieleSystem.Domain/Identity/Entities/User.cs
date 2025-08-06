using MieleSystem.Domain.Common.Base;
using MieleSystem.Domain.Common.Utils;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Events.Admin;
using MieleSystem.Domain.Identity.Events.User;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Entities;

public class User : AggregateRoot, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; } = null!;

    /// Situação do registro do usuário.
    public UserRegistrationSituation RegistrationSituation { get; private set; } =
        UserRegistrationSituation.Pending;

    /// <summary>
    /// Data de expiração da conta, se aplicável.
    /// Usuários com data expirada podem ser automaticamente suspensos.
    /// </summary>
    public DateOnly? ExpiresAt { get; private set; }

    /// <summary>
    /// Coleção de tokens de atualização (refresh tokens) associados a este usuário.
    /// Propriedade de leitura para acesso externo
    /// </summary>
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    /// <summary>
    /// Coleção de sessões OTP (One-Time Password) associadas a este usuário.
    /// Propriedade de leitura para acesso externo
    /// </summary>
    private readonly List<OtpSession> _otpSessions = new();
    public IReadOnlyCollection<OtpSession> OtpSessions => _otpSessions.AsReadOnly();

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    /// Construtor para uso do ORM (EF Core)
    private User()
        : base(Guid.Empty) { }

    /// Construtor com geração automática de PublicId
    public User(string name, Email email, PasswordHash passwordHash, UserRole role)
        : base(Guid.Empty)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;

        AddDomainEvent(new UserRegisteredEvent(PublicId, Role, email));
    }

    /// Construtor com PublicId explícito
    public User(Guid publicId, string name, Email email, PasswordHash passwordHash, UserRole role)
        : base(publicId)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;

        AddDomainEvent(new UserRegisteredEvent(PublicId, Role, email));
    }

    /// <summary>
    /// Define a conta como temporária, com expiração programada.
    /// Deve ser usado para contas do tipo Viewer convidado.
    /// </summary>
    public void MarkAsTemporary(DateOnly expiresAt)
    {
        ExpiresAt = expiresAt;
    }

    // Métodos de domínio para manipulação do usuário
    public void ChangeName(string newName)
    {
        Name = newName;
        AddDomainEvent(new UserUpdatedEvent(PublicId));
    }

    public void ChangePassword(PasswordHash newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        AddDomainEvent(new PasswordChangedEvent(PublicId));
    }

    public void PromoteTo(UserRole role)
    {
        Role = role;
        AddDomainEvent(new UserRoleChangedEvent(PublicId, role));
    }

    public void AddRefreshToken(Token token, DateTime expiresAt)
    {
        var refreshToken = new RefreshToken(token, expiresAt);
        _refreshTokens.Add(refreshToken);
    }

    public void RevokeRefreshToken(Token token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        refreshToken?.Revoke();
    }

    // Método para adicionar uma sessão OTP (One-Time Password)
    public OtpSession AddOtpSession(OtpCode otp)
    {
        var session = new OtpSession(this, otp);
        _otpSessions.Add(session);

        return session;
    }

    // Método para deletar usuários com soft delete
    public void Delete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        AddDomainEvent(new UserDeletedEvent(PublicId));
    }

    // Método para restaurar usuários deletados (soft delete)
    public void Restore()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAt = null;
    }

    // Métodos para gerenciar o registro do usuário
    public void ApproveRegistration()
    {
        if (RegistrationSituation != UserRegistrationSituation.Pending)
            throw new InvalidOperationException("Registro já processado.");

        RegistrationSituation = UserRegistrationSituation.Accepted;

        AddDomainEvent(new UserRegistrationApprovedEvent(PublicId));
    }

    // Método para rejeitar o registro do usuário
    public void DeclineRegistration(string reason)
    {
        if (RegistrationSituation != UserRegistrationSituation.Pending)
            throw new InvalidOperationException("Registro já processado.");

        RegistrationSituation = UserRegistrationSituation.Rejected;

        AddDomainEvent(new UserRegistrationRejectedEvent(PublicId, reason));
    }
}
