using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.LoginUser;

/// <summary>
/// Handler responsável por autenticar o usuário e retornar tokens de acesso e refresh.
/// </summary>
public sealed class LoginUserHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IRefreshTokenHasher refreshTokenHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<LoginUserCommand, Result<LoginUserResult>>
{
    // ... injeções de dependência permanecem as mesmas

    public async Task<Result<LoginUserResult>> Handle(
        LoginUserCommand request,
        CancellationToken ct
    )
    {
        var emailVo = new Email(request.Email);

        var user = await users.GetByEmailAsync(emailVo, ct);
        if (user == null)
        {
            return Result<LoginUserResult>.Failure("Credenciais inválidas.");
        }

        if (!passwordHasher.Verify(user.PasswordHash.Value, request.Password))
        {
            return Result<LoginUserResult>.Failure("Credenciais inválidas.");
        }

        if (user.RegistrationSituation != UserRegistrationSituation.Accepted)
        {
            return Result<LoginUserResult>.Failure(
                "Sua conta ainda não foi aprovada por um administrador."
            );
        }

        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            user.RevokeAllRefreshTokens();
            user.RemoveInvalidRefreshTokens();

            var accessToken = tokenService.GenerateAccessToken(user);
            var plainTextRefreshToken = tokenService.GenerateRefreshToken();
            var refreshTokenHash = refreshTokenHasher.Hash(plainTextRefreshToken);
            var refreshTokenHashVo = new RefreshTokenHash(refreshTokenHash);

            // A validade do token vem da configuração
            var refreshTokenExpiresAt = tokenService.GetRefreshTokenExpiration();
            user.AddRefreshToken(refreshTokenHashVo, refreshTokenExpiresAt);

            users.Update(user);

            await unitOfWork.CommitTransactionAsync(ct);

            var resultDto = new LoginResultDto
            {
                AccessToken = accessToken,
                ExpiresAt = tokenService.GetAccessTokenExpiration(),
            };

            var finalResult = new LoginUserResult(
                resultDto,
                plainTextRefreshToken,
                refreshTokenExpiresAt
            );

            return Result<LoginUserResult>.Success(finalResult);
        }
        catch (Exception) // Captura exceções inesperadas do banco ou de lógica
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            // Logar a exceção aqui é uma boa prática
            return Result<LoginUserResult>.Failure("Ocorreu um erro inesperado durante o login.");
        }
    }
}
