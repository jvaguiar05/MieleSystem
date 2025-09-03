namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class OtpOptions
{
    /// <summary>
    /// Duração de validade do código OTP (em segundos).
    /// </summary>
    public int ExpirationSeconds { get; init; } = default!;
}
