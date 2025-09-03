namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class EmailSenderOptions
{
    public string FromEmail { get; init; } = default!;
    public string FromName { get; init; } = default!;
    public string SmtpHost { get; init; } = default!;
    public int SmtpPort { get; init; } = default!;
    public string SmtpUsername { get; init; } = default!;
    public string SmtpPassword { get; init; } = default!;
    public bool UseSsl { get; init; } = default!;
}
