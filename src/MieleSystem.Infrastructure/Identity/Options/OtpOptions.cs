namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class OtpOptions
{
    /// <summary>
    /// Duração de validade do código OTP (em segundos).
    /// </summary>
    public int ExpirationSeconds { get; init; } =
        Environment.GetEnvironmentVariable("OTP_EXPIRATION") is string expiration
        && int.TryParse(expiration, out var value)
            ? value
            : 300; // padrão: 5 minutos
}
