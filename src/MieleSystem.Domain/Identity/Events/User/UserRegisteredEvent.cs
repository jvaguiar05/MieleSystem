using MieleSystem.Domain.Common.Events;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Events.User;

/// <summary>
/// Evento disparado quando um novo usuário é registrado no sistema.
/// </summary>
public sealed class UserRegisteredEvent(Guid userPublicId, UserRole role, Email email) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
    public UserRole Role { get; } = role;
    public Email Email { get; } = email;
}
