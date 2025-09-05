using MediatR;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Identity.Features.User.Commands.ApproveUserRegistration;

/// <summary>
/// Comando para aprovar o registro de um usuário no sistema.
/// </summary>
public sealed class ApproveUserRegistrationCommand : IRequest<Result<Guid>>
{
    /// <summary>ID público do usuário a ser aprovado.</summary>
    public Guid UserPublicId { get; init; }
}
