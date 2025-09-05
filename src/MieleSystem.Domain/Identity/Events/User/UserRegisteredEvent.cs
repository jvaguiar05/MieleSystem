using MieleSystem.Domain.Common.Events;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Events.User;

/// <summary>
/// Evento disparado quando um novo usuário é registrado no sistema.
/// </summary>
public sealed class UserRegisteredEvent(
    int userId,
    Guid userPublicId,
    UserRole role,
    Email email,
    string userName
) : DomainEvent
{
    public int UserId { get; } = userId;
    public Guid UserPublicId { get; } = userPublicId;
    public UserRole Role { get; } = role;
    public Email Email { get; } = email;
    public string UserName { get; } = userName;
}
