using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Infrastructure.Identity.Services.Email;

/// <summary>
/// Serviço responsável pela validação das configurações de e-mail.
/// </summary>
public sealed class EmailConfigurationValidator
{
    /// <summary>
    /// Valida as configurações do serviço de e-mail.
    /// </summary>
    /// <param name="options">Opções de configuração a serem validadas.</param>
    /// <exception cref="ArgumentException">Quando as configurações são inválidas.</exception>
    public static void ValidateConfiguration(EmailSenderOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.FromEmail))
            throw new ArgumentException(
                "FromEmail não pode ser nulo ou vazio",
                nameof(options.FromEmail)
            );

        if (string.IsNullOrWhiteSpace(options.FromName))
            throw new ArgumentException(
                "FromName não pode ser nulo ou vazio",
                nameof(options.FromName)
            );

        if (string.IsNullOrWhiteSpace(options.SmtpHost))
            throw new ArgumentException(
                "SmtpHost não pode ser nulo ou vazio",
                nameof(options.SmtpHost)
            );

        if (options.SmtpPort <= 0 || options.SmtpPort > 65535)
            throw new ArgumentException(
                "SmtpPort deve estar entre 1 e 65535",
                nameof(options.SmtpPort)
            );

        if (string.IsNullOrWhiteSpace(options.SmtpUsername))
            throw new ArgumentException(
                "SmtpUsername não pode ser nulo ou vazio",
                nameof(options.SmtpUsername)
            );

        if (string.IsNullOrWhiteSpace(options.SmtpPassword))
            throw new ArgumentException(
                "SmtpPassword não pode ser nulo ou vazio",
                nameof(options.SmtpPassword)
            );
    }
}
