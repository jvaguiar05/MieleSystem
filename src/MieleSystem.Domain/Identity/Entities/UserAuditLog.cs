using MieleSystem.Domain.Common.Base;

namespace MieleSystem.Domain.Identity.Entities;

/// <summary>
/// Representa um log de auditoria de ações relacionadas ao usuário.
/// </summary>
public sealed class UserAuditLog : Entity
{
    /// <summary>
    /// Identificador interno (FK para User.Id)
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Identificador público do usuário (exposto em logs e APIs)
    /// </summary>
    public Guid UserPublicId { get; private set; }

    public string Action { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public DateTime OccurredAt { get; private set; }

    private UserAuditLog() { }

    public UserAuditLog(int userId, Guid userPublicId, string action, string email)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        UserPublicId = userPublicId;
        Action = action;
        Email = email;
        OccurredAt = DateTime.UtcNow;
    }
}
