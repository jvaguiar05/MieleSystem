using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.Admin;

/// <summary>
/// Disparado quando um administrador recusa o cadastro de um usuário.
/// </summary>
public sealed class UserRegistrationRejectedEvent(Guid userPublicId, string reason) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
    public string Reason { get; } = reason;
}
