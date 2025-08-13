using MediatR;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Identity.Features.User.Commands.LoginUser;

/// <summary>
/// Comando para autenticação de usuário via e-mail e senha.
/// </summary>
public sealed class LoginUserCommand : IRequest<Result<string>>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
