using DotNetEnv;
using Microsoft.Extensions.Options;
using MieleSystem.Infrastructure.Identity.Options;
using MieleSystem.Presentation.Injection;

var builder = WebApplication.CreateBuilder(args);

// Carrega as variáveis do arquivo .env
Env.Load();

// Adiciona as variáveis de ambiente como uma fonte de configuração
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAPI(builder.Configuration);

var app = builder.Build();

// Executa o log de verificação APENAS se o ambiente for "Development"
if (app.Environment.IsDevelopment())
{
    // =================== LOG DE VERIFICAÇÃO SEGURO ===================
    try
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("--- Iniciando verificação de configurações (Modo Debug) ---");

        // 1. Log das opções de BCrypt
        var bcryptOptions = app.Services.GetRequiredService<IOptions<BCryptOptions>>().Value;
        logger.LogInformation(
            "[Security:BCrypt] WorkFactor: {WorkFactor}",
            bcryptOptions.WorkFactor
        );

        // 2. Log das opções de OTP
        var otpOptions = app.Services.GetRequiredService<IOptions<OtpOptions>>().Value;
        logger.LogInformation(
            "[Security:Otp] ExpirationSeconds: {ExpirationSeconds}",
            otpOptions.ExpirationSeconds
        );

        // 3. Log das opções de JWT (COM O SECRET OCULTO)
        var jwtOptions = app.Services.GetRequiredService<IOptions<JwtOptions>>().Value;
        logger.LogInformation(
            "[Security:Jwt] Issuer: '{Issuer}', Audience: '{Audience}', AccessTokenExpiration: '{AccessTokenExpiration}', Secret: '{Secret}'",
            jwtOptions.Issuer,
            jwtOptions.Audience,
            jwtOptions.AccessTokenExpiration,
            "[OCULTO POR SEGURANÇA]" // <-- Alterado
        );

        // 4. Log das opções de Email (COM A SENHA OCULTA)
        var emailOptions = app.Services.GetRequiredService<IOptions<EmailSenderOptions>>().Value;
        logger.LogInformation(
            "[Email] FromEmail: '{FromEmail}', FromName: '{FromName}', SmtpHost: '{SmtpHost}', SmtpPort: {SmtpPort}, UseSsl: {UseSsl}, SmtpUsername: '{SmtpUsername}', SmtpPassword: '{SmtpPassword}'",
            emailOptions.FromEmail,
            emailOptions.FromName,
            emailOptions.SmtpHost,
            emailOptions.SmtpPort,
            emailOptions.UseSsl,
            emailOptions.SmtpUsername,
            "[OCULTO POR SEGURANÇA]" // <-- Alterado
        );

        logger.LogInformation("--- Verificação de configurações concluída ✅ ---");
    }
    catch (OptionsValidationException ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(
            ex,
            "Erro de validação nas configurações da aplicação! Verifique seu appsettings ou .env."
        );
        return; // Encerra a aplicação
    }
    // ======================================================================================
}

app.UseAPI();

app.Run();
