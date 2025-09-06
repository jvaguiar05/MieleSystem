using Microsoft.Extensions.Options;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Presentation.Utils;

/// <summary>
/// Classe auxiliar para logging de variáveis de configuração de forma segura e organizada.
/// Esta classe centraliza a lógica de logging para todas as opções de configuração usadas na aplicação.
/// </summary>
public static class ConfigurationLogger
{
    /// <summary>
    /// Logs todas as opções de configuração de forma segura, escondendo informações sensíveis.
    /// Este método deve ser chamado apenas no ambiente de desenvolvimento.
    /// </summary>
    /// <param name="logger">A instância do logger para usar para logging</param>
    /// <param name="serviceProvider">O provedor de serviços para resolver opções de configuração</param>
    public static void LogConfigurationVariables(ILogger logger, IServiceProvider serviceProvider)
    {
        try
        {
            logger.LogInformation("--- Iniciando verificação de configurações (Modo Debug) ---");

            // Log BCrypt options (opções de configuração do BCrypt)
            LogBCryptOptions(logger, serviceProvider);

            // Log OTP options (opções de configuração do OTP)
            LogOtpOptions(logger, serviceProvider);

            // Log JWT options (com segredo escondido para segurança)
            LogJwtOptions(logger, serviceProvider);

            // Log Email options (com senha escondida para segurança)
            LogEmailOptions(logger, serviceProvider);

            logger.LogInformation("--- Verificação de configurações concluída ✅ ---");
        }
        catch (OptionsValidationException ex)
        {
            logger.LogCritical(
                ex,
                "Erro de validação nas configurações da aplicação! Verifique seu appsettings ou .env."
            );
            throw; // Re-throw para ser tratado pelo chamador
        }
    }

    /// <summary>
    /// Logs opções de configuração do BCrypt.
    /// </summary>
    private static void LogBCryptOptions(ILogger logger, IServiceProvider serviceProvider)
    {
        var bcryptOptions = serviceProvider.GetRequiredService<IOptions<BCryptOptions>>().Value;
        logger.LogInformation(
            "[Security:BCrypt] WorkFactor: {WorkFactor}",
            bcryptOptions.WorkFactor
        );
    }

    /// <summary>
    /// Logs opções de configuração do OTP.
    /// </summary>
    private static void LogOtpOptions(ILogger logger, IServiceProvider serviceProvider)
    {
        var otpOptions = serviceProvider.GetRequiredService<IOptions<OtpOptions>>().Value;
        logger.LogInformation(
            "[Security:Otp] ExpirationSeconds: {ExpirationSeconds}",
            otpOptions.ExpirationSeconds
        );
    }

    /// <summary>
    /// Logs opções de configuração do JWT com o segredo escondido para segurança.
    /// </summary>
    private static void LogJwtOptions(ILogger logger, IServiceProvider serviceProvider)
    {
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
        logger.LogInformation(
            "[Security:Jwt] Issuer: '{Issuer}', Audience: '{Audience}', AccessTokenExpiration: '{AccessTokenExpiration}', Secret: '{Secret}', RefreshTokenExpirationInDays: {RefreshTokenExpirationInDays}",
            jwtOptions.Issuer,
            jwtOptions.Audience,
            jwtOptions.AccessTokenExpiration,
            "[OCULTO POR SEGURANÇA]",
            jwtOptions.RefreshTokenExpirationInDays
        );
    }

    /// <summary>
    /// Logs opções de configuração do Email com a senha escondida para segurança.
    /// </summary>
    private static void LogEmailOptions(ILogger logger, IServiceProvider serviceProvider)
    {
        var emailOptions = serviceProvider.GetRequiredService<IOptions<EmailSenderOptions>>().Value;
        logger.LogInformation(
            "[Email] FromEmail: '{FromEmail}', FromName: '{FromName}', SmtpHost: '{SmtpHost}', SmtpPort: {SmtpPort}, UseSsl: {UseSsl}, SmtpUsername: '{SmtpUsername}', SmtpPassword: '{SmtpPassword}'",
            emailOptions.FromEmail,
            emailOptions.FromName,
            emailOptions.SmtpHost,
            emailOptions.SmtpPort,
            emailOptions.UseSsl,
            emailOptions.SmtpUsername,
            "[OCULTO POR SEGURANÇA]"
        );
    }

    /// <summary>
    /// Logs um objeto de configuração específico com todas as suas propriedades.
    /// Útil para logging de objetos de configuração personalizados.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de configuração</typeparam>
    /// <param name="logger">A instância do logger</param>
    /// <param name="configurationName">O nome da seção de configuração</param>
    /// <param name="configuration">O objeto de configuração para logging</param>
    /// <param name="sensitiveProperties">Propriedades que devem ser escondidas (opcional)</param>
    public static void LogConfigurationObject<T>(
        ILogger logger,
        string configurationName,
        T configuration,
        params string[] sensitiveProperties
    )
    {
        var properties = typeof(T).GetProperties();
        var logMessage = $"[{configurationName}] ";
        var logArgs = new List<object>();

        foreach (var property in properties)
        {
            var value = property.GetValue(configuration);
            var isSensitive = sensitiveProperties.Contains(property.Name);
            var displayValue = isSensitive ? "[OCULTO POR SEGURANÇA]" : value?.ToString() ?? "null";

            logMessage += $"{property.Name}: '{{{property.Name}}}', ";
            logArgs.Add(displayValue);
        }

        // Remove o último comma e espaço
        logMessage = logMessage.TrimEnd(' ', ',');

        logger.LogInformation(logMessage, logArgs.ToArray());
    }

    /// <summary>
    /// Logs variáveis de ambiente de forma segura.
    /// </summary>
    /// <param name="logger">A instância do logger</param>
    /// <param name="environmentVariables">Dicionário de variáveis de ambiente</param>
    /// <param name="sensitiveKeys">Chaves que devem ser escondidas (opcional)</param>
    public static void LogEnvironmentVariables(
        ILogger logger,
        IDictionary<string, string> environmentVariables,
        params string[] sensitiveKeys
    )
    {
        logger.LogInformation("--- Environment Variables ---");

        foreach (var kvp in environmentVariables.OrderBy(x => x.Key))
        {
            var isSensitive = sensitiveKeys.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase);
            var displayValue = isSensitive ? "[OCULTO POR SEGURANÇA]" : kvp.Value;

            logger.LogInformation("[ENV] {Key}: {Value}", kvp.Key, displayValue);
        }

        logger.LogInformation("--- Environment Variables End ---");
    }
}
