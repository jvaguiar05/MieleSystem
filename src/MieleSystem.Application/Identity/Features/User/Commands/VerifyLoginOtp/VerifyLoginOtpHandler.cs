using MediatR;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.VerifyLoginOtp;

/// <summary>
/// Handler responsável por verificar o código OTP e completar o processo de autenticação.
/// </summary>
public sealed class VerifyLoginOtpHandler(
    IUserRepository users,
    ITokenService tokenService,
    IRefreshTokenHasher refreshTokenHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<VerifyLoginOtpCommand, Result<VerifyLoginOtpResult>>
{
    private readonly IUserRepository _users = users;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokenHasher _refreshTokenHasher = refreshTokenHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<VerifyLoginOtpResult>> Handle(
        VerifyLoginOtpCommand request,
        CancellationToken ct
    )
    {
        var emailVo = new Email(request.Email);

        var user = await _users.GetByEmailAsync(emailVo, ct);
        if (user == null)
            return Result<VerifyLoginOtpResult>.Failure(
                Error.Unauthorized("Sessão inválida ou expirada.")
            );

        if (user.RegistrationSituation != UserRegistrationSituation.Accepted)
        {
            var failedConnectionLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "OTP verification failed: account not approved"
            );
            failedConnectionLog.MarkOtpRequired("Account verification pending");

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<VerifyLoginOtpResult>.Failure(
                Error.Forbidden("Sua conta ainda não foi aprovada por um administrador.")
            );
        }

        if (!user.TryConsumeOtp(request.OtpCode))
        {
            var invalidOtpConnectionLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "OTP verification failed: invalid or expired code"
            );
            invalidOtpConnectionLog.MarkOtpRequired("OTP verification attempted");

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<VerifyLoginOtpResult>.Failure(
                Error.Unauthorized("Código OTP inválido ou expirado.")
            );
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

            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();
            user.AddRefreshToken(refreshTokenHashVo, refreshTokenExpiresAt);

            var successConnectionLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "Login successful with OTP verification"
            );
            successConnectionLog.MarkAsSuccessful();
            successConnectionLog.MarkOtpRequired("OTP verification completed successfully");

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);

            var resultDto = new LoginResultDto
            {
                AccessToken = accessToken,
                ExpiresAt = _tokenService.GetAccessTokenExpiration(),
            };

            var finalResult = new VerifyLoginOtpResult(
                resultDto,
                plainTextRefreshToken,
                refreshTokenExpiresAt
            );

            return Result<VerifyLoginOtpResult>.Success(finalResult);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);

            var infrastructureFailureLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: $"OTP verification failed: infrastructure error - {ex.Message}"
            );
            infrastructureFailureLog.MarkOtpRequired("OTP verification infrastructure error");

            try
            {
                _users.Update(user);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch { }

            return Result<VerifyLoginOtpResult>.Failure(
                Error.Infrastructure(
                    "otp_verification.infrastructure_error",
                    "Ocorreu um erro inesperado durante a verificação do OTP.",
                    details: ex.CreateExceptionDetails("VerifyLoginOtp")
                )
            );
        }
    }
}
