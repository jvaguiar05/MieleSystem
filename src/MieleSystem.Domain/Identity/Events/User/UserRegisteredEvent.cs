using MieleSystem.Domain.Common.Events;
using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Domain.Identity.Events.User;

/// <summary>
/// Evento disparado quando um novo usuário é registrado no sistema.
/// </summary>
public sealed class UserRegisteredEvent(Guid userPublicId, UserRole role, string email)
    : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
    public UserRole Role { get; } = role;
    public string Email { get; } = email;
}
