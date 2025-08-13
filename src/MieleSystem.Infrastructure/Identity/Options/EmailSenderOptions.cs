namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class EmailSenderOptions
{
    public string FromEmail { get; init; } = default!;
    public string FromName { get; init; } = "MieleSystem";
    public string SmtpHost { get; init; } = default!;
    public int SmtpPort { get; init; }
    public string SmtpUsername { get; init; } = default!;
    public string SmtpPassword { get; init; } = default!;
    public bool UseSsl { get; init; } = true;
}
