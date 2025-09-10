using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Application.Identity.Services.Authentication;

/// <summary>
/// Serviço para verificação e validação de códigos OTP.
/// </summary>
public interface IOtpVerificationService
{
    /// <summary>
    /// Verifica se um código OTP é válido para um usuário e propósito específicos.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="code">Código OTP a ser verificado.</param>
    /// <param name="purpose">Propósito do OTP (Login, PasswordRecovery, etc.).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Resultado da verificação, incluindo validade e detalhes da sessão.</returns>
    Task<OtpVerificationResult> VerifyOtpAsync(
        int userId,
        string code,
        OtpPurpose purpose,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém a sessão OTP ativa mais recente para um usuário e propósito.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="purpose">Propósito do OTP.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>DTO da sessão OTP ativa ou nulo se não existir.</returns>
    Task<OtpSessionDto?> GetActiveSessionAsync(
        int userId,
        OtpPurpose purpose,
        CancellationToken ct = default
    );

    /// <summary>
    /// Verifica se existe uma sessão OTP ativa para um usuário e propósito específicos.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="purpose">Propósito do OTP.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Verdadeiro se existir uma sessão ativa, falso caso contrário.</returns
    Task<bool> HasActiveSessionAsync(
        int userId,
        OtpPurpose purpose,
        CancellationToken ct = default
    );
}

/// <summary>
/// Resultado da verificação de OTP.
/// </summary>
public sealed record OtpVerificationResult(
    bool IsValid,
    OtpSessionDto? Session,
    string? ErrorMessage = null
);
