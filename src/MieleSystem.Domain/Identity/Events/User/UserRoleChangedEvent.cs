using MieleSystem.Domain.Common.Events;
using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Domain.Identity.Events.User;

/// <summary>
/// Evento disparado quando o papel (role) de um usuário é alterado.
/// </summary>
public sealed class UserRoleChangedEvent(Guid userPublicId, UserRole newRole) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
    public UserRole NewRole { get; } = newRole;
}
