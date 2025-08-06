using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.Auth;

/// <summary>
/// Evento disparado quando um código OTP é solicitado por um usuário.
/// </summary>
public sealed class OtpCodeRequestedEvent(Guid userPublicId) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
}
