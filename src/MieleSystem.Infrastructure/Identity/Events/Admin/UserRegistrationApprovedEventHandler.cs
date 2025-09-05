using MediatR;
using Microsoft.Extensions.Logging;
using MieleSystem.Application.Common.Events;
using MieleSystem.Application.Identity.Services.Email;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Events.Admin;
using MieleSystem.Domain.Identity.Repositories;

namespace MieleSystem.Infrastructure.Identity.Events.Admin;

/// <summary>
/// Handler responsável por reagir ao evento UserRegistrationApprovedEvent.
/// Envia e-mail de aprovação ao usuário e registra log de auditoria.
/// </summary>
public sealed class UserRegistrationApprovedEventHandler(
    IAccountEmailService emailService,
    IUserAuditLogRepository auditLogRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserRegistrationApprovedEventHandler> logger
) : INotificationHandler<EventNotification<UserRegistrationApprovedEvent>>
{
    private readonly IAccountEmailService _emailService = emailService;
    private readonly IUserAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UserRegistrationApprovedEventHandler> _logger = logger;

    public async Task Handle(
        EventNotification<UserRegistrationApprovedEvent> notification,
        CancellationToken cancellationToken
    )
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "📩 Processando aprovação de registro para usuário: {UserPublicId}",
            domainEvent.UserPublicId
        );

        // Busca o usuário para obter informações de email e nome
        var user = await _userRepository.GetByPublicIdAsync(
            domainEvent.UserPublicId,
            cancellationToken
        );
        if (user is null)
        {
            _logger.LogWarning(
                "⚠️ Usuário não encontrado para aprovação: {UserPublicId}",
                domainEvent.UserPublicId
            );
            return;
        }

        _logger.LogInformation(
            "📩 Enviando e-mail de aprovação para: {Email} (UserId: {UserId})",
            user.Email,
            domainEvent.UserPublicId
        );

        await _emailService.SendRegistrationApprovedAsync(user.Email, user.Name, cancellationToken);

        _logger.LogInformation(
            "✅ E-mail de aprovação enviado com sucesso para: {Email}",
            user.Email
        );

        var log = new UserAuditLog(
            user.Id,
            domainEvent.UserPublicId,
            action: "UserRegistrationApproved",
            email: user.Email.ToString()
        );

        await _auditLogRepository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "🗒️ Log de auditoria registrado para aprovação do usuário: {UserId}",
            domainEvent.UserPublicId
        );
    }
}
