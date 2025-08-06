namespace MieleSystem.Domain.Identity.Events.Auth;

/// <summary>
/// Evento disparado quando um usuário realiza logout.
/// Pode ser usado para registrar auditoria, limpar sessões, etc.
/// </summary>
public class UserLoggedOutEvent(Guid userPublicId)
{
    public Guid UserPublicId { get; } = userPublicId;
}
