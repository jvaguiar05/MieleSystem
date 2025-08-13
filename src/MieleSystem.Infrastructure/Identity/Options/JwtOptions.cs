namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class JwtOptions
{
    public string SecretKey { get; init; } = "KAJSDHJKSAHQJ1H231KJH21KJ3HE98UQ9DHQSDJASKDHAKJSH123";
    public string Issuer { get; init; } = "MieleSystem";
    public string Audience { get; init; } = "MieleSystemClient";
    public int AccessTokenExpirationMinutes { get; init; } = 60;
}
