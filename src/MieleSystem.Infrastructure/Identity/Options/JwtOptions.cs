namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class JwtOptions
{
    public string SecretKey { get; init; } =
        Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default_secret_key";
    public string Issuer { get; init; } =
        Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MieleSystem";
    public string Audience { get; init; } =
        Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MieleSystemClient";
    public int AccessTokenExpirationMinutes { get; init; } =
        int.TryParse(
            Environment.GetEnvironmentVariable("JWT_ACCESS_EXPIRATION"),
            out var expiration
        )
            ? expiration
            : 60;
}
