using MediatR;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.Services.Email;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.RegenerateOtp;

/// <summary>
/// Handler responsável por regenerar códigos OTP expirados.
/// Controla limites de regeneração e suspensão automática por falhas.
/// </summary>
public sealed class RegenerateOtpHandler(
    IUserRepository users,
    IOtpService otpService,
    IAccountEmailService emailService,
    IUnitOfWork unitOfWork
) : IRequestHandler<RegenerateOtpCommand, Result<RegenerateOtpResult>>
{
    private readonly IUserRepository _users = users;
    private readonly IOtpService _otpService = otpService;
    private readonly IAccountEmailService _emailService = emailService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RegenerateOtpResult>> Handle(
        RegenerateOtpCommand request,
        CancellationToken ct
    )
    {
        var emailVo = new Email(request.Email);

        var user = await _users.GetByEmailAsync(emailVo, ct);
        if (user == null)
            return Result<RegenerateOtpResult>.Failure(
                Error.NotFound("user.not_found", "Usuário não encontrado.")
            );

        // Verificar se a conta está suspensa
        if (user.IsSuspended)
        {
            var suspendedLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "OTP regeneration blocked: account suspended"
            );

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<RegenerateOtpResult>.Failure(
                Error.Forbidden("Conta suspensa. Entre em contato com o suporte.")
            );
        }

        // Verificar se deve ser suspenso por excesso de falhas
        if (user.ShouldBeSuspendedForOtpFailures(OtpPurpose.Login))
        {
            user.SuspendAccount("Excesso de falhas na verificação de OTP");

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<RegenerateOtpResult>.Failure(
                Error.Forbidden("Conta suspensa automaticamente por excesso de tentativas de OTP.")
            );
        }

        // Tentar regenerar OTP
        var newOtpCode = _otpService.Generate();
        var regenerated = user.TryRegenerateOtp(newOtpCode, OtpPurpose.Login);

        if (!regenerated)
        {
            var failedLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "OTP regeneration failed: limit exceeded"
            );

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<RegenerateOtpResult>.Failure(
                Error.Validation(
                    "otp.regeneration_limit",
                    "Limite de regenerações de OTP excedido. Tente fazer login novamente mais tarde."
                )
            );
        }

        try
        {
            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            await _emailService.SendOtpAsync(user.Email, newOtpCode.Code, newOtpCode.ExpiresAt, ct);

            var successLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "OTP regenerated successfully"
            );

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            var latestSession = user
                .OtpSessions.Where(s => s.Purpose == OtpPurpose.Login)
                .OrderByDescending(s => s.CreatedAtUtc)
                .First();

            var attemptsRemaining = Math.Max(0, 3 - latestSession.RegenerationAttempts);

            var result = new RegenerateOtpResult
            {
                Success = true,
                Message = "Novo código OTP enviado para seu e-mail.",
                NewExpirationTime = newOtpCode.ExpiresAt,
                AttemptsRemaining = attemptsRemaining,
            };

            return Result<RegenerateOtpResult>.Success(result);
        }
        catch (Exception ex)
        {
            var errorLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: $"OTP regeneration error: {ex.Message}"
            );

            try
            {
                _users.Update(user);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch { }

            return Result<RegenerateOtpResult>.Failure(
                Error.Infrastructure(
                    "otp.regeneration_error",
                    "Erro ao regenerar código OTP.",
                    details: ex.CreateExceptionDetails("RegenerateOtp")
                )
            );
        }
    }
}
