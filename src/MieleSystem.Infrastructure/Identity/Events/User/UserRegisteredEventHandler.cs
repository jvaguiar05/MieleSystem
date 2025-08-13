using MediatR;
using Microsoft.Extensions.Logging;
using MieleSystem.Application.Common.Events;
using MieleSystem.Application.Identity.Services;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Events.User;
using MieleSystem.Domain.Identity.Repositories;

namespace MieleSystem.Infrastructure.Identity.Events.User;

/// <summary>
/// Handler responsável por reagir ao evento UserRegisteredEvent.
/// Envia e-mail de boas-vindas ao novo usuário e registra log de auditoria.
/// </summary>
public sealed class UserRegisteredEventHandler(
    IAccountEmailService emailService,
    IUserAuditLogRepository auditLogRepository,
    ILogger<UserRegisteredEventHandler> logger
) : INotificationHandler<EventNotification<UserRegisteredEvent>>
{
    private readonly IAccountEmailService _emailService = emailService;
    private readonly IUserAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly ILogger<UserRegisteredEventHandler> _logger = logger;

    public async Task Handle(
        EventNotification<UserRegisteredEvent> notification,
        CancellationToken cancellationToken
    )
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "📩 Disparando e-mail de boas-vindas para: {Email} (UserId: {UserId})",
            domainEvent.Email,
            domainEvent.UserPublicId
        );

        await _emailService.SendWelcomeAsync(domainEvent.Email, "Usuário Miele", cancellationToken);

        _logger.LogInformation("✅ E-mail enviado com sucesso para: {Email}", domainEvent.Email);

        var log = new UserAuditLog(
            domainEvent.UserId,
            domainEvent.UserPublicId,
            action: "UserRegistered",
            email: domainEvent.Email.ToString()
        );

        await _auditLogRepository.AddAsync(log, cancellationToken);

        _logger.LogInformation(
            "🗒️ Log de auditoria registrado para o usuário: {UserId}",
            domainEvent.UserPublicId
        );
    }
}
