using MieleSystem.Domain.Common.Events;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Domain.Identity.Events.Admin;

/// <summary>
/// Disparado quando um usuário é registrado mas requer aprovação administrativa.
/// </summary>
public sealed class UserRegistrationPendingEvent(Guid userPublicId, Email email) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
    public Email Email { get; } = email;
}
