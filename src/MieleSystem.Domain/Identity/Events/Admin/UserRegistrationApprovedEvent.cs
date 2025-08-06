using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.Admin;

/// <summary>
/// Disparado quando um administrador aprova o cadastro de um usuário.
/// </summary>
public sealed class UserRegistrationApprovedEvent(Guid userPublicId) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
}
