using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.Auth;

/// <summary>
/// Evento disparado quando um usuário realiza login com sucesso.
/// Pode ser usado para gerar e persistir RefreshTokens, registrar auditoria, etc.
/// </summary>
public sealed class UserLoggedInEvent(Guid userPublicId) : DomainEvent
{
    public Guid UserPublicId { get; } = userPublicId;
}
