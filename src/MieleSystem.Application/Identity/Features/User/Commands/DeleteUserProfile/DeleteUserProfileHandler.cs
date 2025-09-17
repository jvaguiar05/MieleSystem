using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Repositories;

namespace MieleSystem.Application.Identity.Features.User.Commands.DeleteUserProfile;

/// <summary>
/// Handler responsável por processar a exclusão do perfil do próprio usuário.
/// </summary>
public sealed class DeleteUserProfileHandler(
    IUserRepository userRepository,
    IUserAuditLogRepository auditLogRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteUserProfileCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(
        DeleteUserProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        // Busca o usuário pelo ID público extraído do token
        var user = await _userRepository.GetByPublicIdAsync(
            request.CurrentUserPublicId,
            cancellationToken
        );

        if (user == null)
            return Result<bool>.Failure(
                Error.NotFound("user.not_found", "Usuário não encontrado.")
            );

        if (user.IsDeleted)
            return Result<bool>.Failure(
                Error.Validation("user.already_deleted", "O perfil já foi deletado.")
            );

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Log de auditoria para CRUD operations
            var auditLog = new UserAuditLog(
                user.Id,
                user.PublicId,
                "DELETE_PROFILE",
                user.Email.Value
            );
            await _auditLogRepository.AddAsync(auditLog, cancellationToken);

            // Soft delete do usuário
            user.Delete();

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            return Result<bool>.Failure(
                Error.Infrastructure(
                    "user.deletion_failed",
                    "Ocorreu um erro ao deletar o perfil.",
                    details: ex.Message
                )
            );
        }
    }
}
