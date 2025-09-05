using FluentValidation;

namespace MieleSystem.Application.Identity.Features.User.Commands.ApproveUserRegistration;

/// <summary>
/// Validador para o comando de aprovação de registro de usuário.
/// </summary>
public sealed class ApproveUserRegistrationValidator
    : AbstractValidator<ApproveUserRegistrationCommand>
{
    public ApproveUserRegistrationValidator()
    {
        RuleFor(x => x.UserPublicId).NotEmpty().WithMessage("ID do usuário é obrigatório.");
    }
}
