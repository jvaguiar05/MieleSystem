using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.User;

/// <summary>
/// Evento disparado quando a senha de um usuário é alterada.
/// </summary>
public sealed class PasswordChangedEvent(Guid userPublicId) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
}
