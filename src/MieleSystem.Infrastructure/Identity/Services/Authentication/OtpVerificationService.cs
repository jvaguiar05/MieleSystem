using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Services.Authentication;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Infrastructure.Identity.Services.Authentication;

/// <summary>
/// Implementação do serviço de verificação de OTP.
/// </summary>
public sealed class OtpVerificationService(IOtpSessionReadStore otpSessionStore)
    : IOtpVerificationService
{
    private readonly IOtpSessionReadStore _otpSessionStore = otpSessionStore;

    public async Task<OtpVerificationResult> VerifyOtpAsync(
        int userId,
        string code,
        OtpPurpose purpose,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrWhiteSpace(code))
            return new OtpVerificationResult(false, null, "Código OTP não informado.");

        var session = await _otpSessionStore.GetActiveSessionByCodeAsync(userId, code, purpose, ct);

        if (session == null)
            return new OtpVerificationResult(false, null, "Código OTP inválido ou expirado.");

        // Verifica se a sessão ainda está dentro do prazo de validade
        if (session.ExpiresAtUtc <= DateTime.UtcNow)
            return new OtpVerificationResult(false, session, "Código OTP expirado.");

        // Verifica se a sessão já foi usada
        if (session.IsUsed)
            return new OtpVerificationResult(false, session, "Código OTP já foi utilizado.");

        return new OtpVerificationResult(true, session);
    }

    public async Task<OtpSessionDto?> GetActiveSessionAsync(
        int userId,
        OtpPurpose purpose,
        CancellationToken ct = default
    )
    {
        return await _otpSessionStore.GetLatestActiveSessionAsync(userId, purpose, ct);
    }

    public async Task<bool> HasActiveSessionAsync(
        int userId,
        OtpPurpose purpose,
        CancellationToken ct = default
    )
    {
        var session = await _otpSessionStore.GetLatestActiveSessionAsync(userId, purpose, ct);
        return session != null && !session.IsUsed && session.ExpiresAtUtc > DateTime.UtcNow;
    }
}
