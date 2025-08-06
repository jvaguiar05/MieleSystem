using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.User;

/// <summary>
/// Evento disparado quando um usuário é excluído do sistema.
/// </summary>
public sealed class UserDeletedEvent(Guid userPublicId) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
}
