using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.User;

/// <summary>
/// Evento disparado quando um usuário é atualizado no sistema.
/// </summary>
public sealed class UserUpdatedEvent(Guid userPublicId) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
}
