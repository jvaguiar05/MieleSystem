namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class EmailSenderOptions
{
    public string FromEmail { get; init; } =
        Environment.GetEnvironmentVariable("EMAIL_FROM") ?? "noreply@mielesystem.com";
    public string FromName { get; init; } =
        Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "MieleSystem";
    public string SmtpHost { get; init; } =
        Environment.GetEnvironmentVariable("EMAIL_HOST") ?? default!;
    public int SmtpPort { get; init; } =
        int.TryParse(Environment.GetEnvironmentVariable("EMAIL_PORT"), out var port)
            ? port
            : default!;
    public string SmtpUsername { get; init; } =
        Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? default!;
    public string SmtpPassword { get; init; } =
        Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? default!;
    public bool UseSsl { get; init; } =
        !bool.TryParse(Environment.GetEnvironmentVariable("EMAIL_USE_SSL"), out var useSsl)
        || useSsl;
}
