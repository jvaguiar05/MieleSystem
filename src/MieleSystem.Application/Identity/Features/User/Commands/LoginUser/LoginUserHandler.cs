using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Domain.Identity.Enums;
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
        // Normaliza e valida VOs básicos
        var emailVo = new Email(request.Email);

        // Busca usuário por email
        var user =
            await _users.GetByEmailAsync(emailVo, ct)
            ?? throw new UnauthorizedAccessException(
                $"Usuário com email {emailVo} não encontrado na base de dados."
            );

        // Verifica a senha
        var isPasswordValid = _passwordHasher.Verify(user.PasswordHash.Value, request.Password);

        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Senha inválida. Tente novamente.");

        // Verifica a situação da conta
        if (user.RegistrationSituation != UserRegistrationSituation.Accepted)
            throw new UnauthorizedAccessException(
                "Conta não autorizada. Aguarde a validação ou entre em contato com o suporte."
            );

        // Gera token JWT
        var token = _tokenService.GenerateAccessToken(user);

        // Retorna o token gerado
        return Result<string>.Success(token);
    }
}
