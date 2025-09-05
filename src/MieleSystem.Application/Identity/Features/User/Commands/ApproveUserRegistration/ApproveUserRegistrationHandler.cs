using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Repositories;

namespace MieleSystem.Application.Identity.Features.User.Commands.ApproveUserRegistration;

/// <summary>
/// Handler responsável por aprovar o registro de um usuário no sistema.
/// Verifica se o usuário existe, se está pendente de aprovação e aprova o registro.
/// </summary>
public sealed class ApproveUserRegistrationHandler(IUserRepository users, IUnitOfWork uow)
    : IRequestHandler<ApproveUserRegistrationCommand, Result<Guid>>
{
    private readonly IUserRepository _users = users;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<Guid>> Handle(
        ApproveUserRegistrationCommand request,
        CancellationToken ct
    )
    {
        // Busca o usuário pelo ID público
        var user = await _users.GetByPublicIdAsync(request.UserPublicId, ct);
        if (user is null)
            return Result<Guid>.Failure("Usuário não encontrado.");

        // Verifica se o usuário está pendente de aprovação
        if (user.RegistrationSituation != Domain.Identity.Enums.UserRegistrationSituation.Pending)
            return Result<Guid>.Failure("Usuário já foi processado anteriormente.");

        // Aprova o registro do usuário
        user.ApproveRegistration();

        // Salva as alterações
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(user.PublicId);
    }
}
