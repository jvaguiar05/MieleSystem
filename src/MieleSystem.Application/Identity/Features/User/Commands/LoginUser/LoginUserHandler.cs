using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.LoginUser;

/// <summary>
/// Handler responsável por autenticar o usuário e retornar um JWT válido.
/// </summary>
public sealed class LoginUserHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenService tokenService
) : IRequestHandler<LoginUserCommand, Result<string>>
{
    private readonly IUserRepository _users = users;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var emailVo = new Email(request.Email);

        var user = await _users.GetByEmailAsync(emailVo, ct);
        if (user is null)
            return Result<string>.Failure("Usuário ou senha inválidos.");

        var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash.Value);

        if (!isPasswordValid)
            return Result<string>.Failure("Usuário ou senha inválidos.");

        var token = _tokenService.GenerateAccessToken(user);

        return Result<string>.Success(token);
    }
}
