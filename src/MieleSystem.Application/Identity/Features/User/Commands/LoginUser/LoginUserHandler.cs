using MediatR;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Services;
using MieleSystem.Application.Identity.Services.Email;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.LoginUser;

/// <summary>
/// Handler responsável por autenticar o usuário e retornar tokens de acesso e refresh.
/// Inclui lógica condicional para OTP quando necessário.
/// </summary>
public sealed class LoginUserHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IRefreshTokenHasher refreshTokenHasher,
    IUnitOfWork unitOfWork,
    IAuthenticationContextService authContextService,
    IOtpService otpService,
    IAccountEmailService emailService
) : IRequestHandler<LoginUserCommand, Result<LoginUserResult>>
{
    private readonly IUserRepository _users = users;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokenHasher _refreshTokenHasher = refreshTokenHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthenticationContextService _authContextService = authContextService;
    private readonly IOtpService _otpService = otpService;
    private readonly IAccountEmailService _emailService = emailService;

    public async Task<Result<LoginUserResult>> Handle(
        LoginUserCommand request,
        CancellationToken ct
    )
    {
        var emailVo = new Email(request.Email);

        var user = await _users.GetByEmailAsync(emailVo, ct);
        if (user == null)
            return Result<LoginUserResult>.Failure(
                Error.Unauthorized(
                    $"Credenciais inválidas: usuário {request.Email} não encontrado."
                )
            );

        if (!_passwordHasher.Verify(user.PasswordHash.Value, request.Password))
            return Result<LoginUserResult>.Failure(
                Error.Unauthorized(
                    $"Credenciais inválidas: senha incorreta para usuário {request.Email}."
                )
            );

        if (user.RegistrationSituation != UserRegistrationSituation.Accepted)
            return Result<LoginUserResult>.Failure(
                Error.Forbidden("Sua conta ainda não foi aprovada por um administrador.")
            );

        // Checa se OTP é necessário para este login
        var clientInfo = new AuthenticationClientInfo(
            request.ClientIp,
            request.UserAgent,
            request.DeviceId
        );

        if (await _authContextService.IsOtpRequiredAsync(user, clientInfo))
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var otpCode = _otpService.Generate();
                var otpSession = user.AddOtpSession(otpCode, OtpPurpose.Login);

                _users.Update(user);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);

                await _emailService.SendOtpAsync(user.Email, otpCode.Code, otpCode.ExpiresAt, ct);

                return Result<LoginUserResult>.Failure(
                    Error.OtpRequired(
                        "Verificação OTP necessária. Um código foi enviado para seu e-mail.",
                        details: new { RequiresOtp = true, Email = user.Email.Value }
                    )
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                return Result<LoginUserResult>.Failure(
                    Error.Infrastructure(
                        "login.otp_generation_error",
                        "Erro ao gerar código OTP.",
                        details: ex.CreateExceptionDetails("GenerateOTP")
                    )
                );
            }
        }

        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            user.RevokeAllRefreshTokens();
            user.RemoveInvalidRefreshTokens();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var plainTextRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _refreshTokenHasher.Hash(plainTextRefreshToken);
            var refreshTokenHashVo = new RefreshTokenHash(refreshTokenHash);

            // A validade do token vem da configuração
            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();
            user.AddRefreshToken(refreshTokenHashVo, refreshTokenExpiresAt);

            _users.Update(user);

            await _unitOfWork.CommitTransactionAsync(ct);

            var resultDto = new LoginResultDto
            {
                AccessToken = accessToken,
                ExpiresAt = _tokenService.GetAccessTokenExpiration(),
            };

            var finalResult = new LoginUserResult(
                resultDto,
                plainTextRefreshToken,
                refreshTokenExpiresAt
            );

            return Result<LoginUserResult>.Success(finalResult);
        }
        catch (Exception ex) // Captura exceções inesperadas do banco ou de lógica
        {
            await _unitOfWork.RollbackTransactionAsync(ct);

            return Result<LoginUserResult>.Failure(
                Error.Infrastructure(
                    "login.infrastructure_error",
                    "Ocorreu um erro inesperado durante o login.",
                    details: ex.CreateExceptionDetails("LoginUser")
                )
            );
        }
    }
}
