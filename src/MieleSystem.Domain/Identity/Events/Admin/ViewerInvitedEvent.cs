using MieleSystem.Domain.Common.Events;

namespace MieleSystem.Domain.Identity.Events.Admin;

/// <summary>
/// Disparado quando um usuário é convidado para ser um visualizador.
/// Este evento pode ser usado para enviar um e-mail de convite ou registrar auditoria.
/// O convite pode incluir informações como o ID público do usuário, e-mail, nome de usuário, senha e data de expiração do convite.
/// O evento é imutável e contém todas as informações necessárias para processar o convite.
/// </summary>
public sealed class ViewerUserInvitedEvent(
    Guid publicUserId,
    string email,
    string username,
    string password,
    DateOnly? expiresAt
) : DomainEvent
{
    public Guid PublicUserId { get; } = publicUserId;
    public string Email { get; } = email;
    public string Username { get; } = username;
    public string Password { get; } = password;
    public DateOnly? ExpiresAt { get; } = expiresAt;
}
