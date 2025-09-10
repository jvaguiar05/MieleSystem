using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MieleSystem.Application.Identity.Services.Authentication;
using MieleSystem.Application.Identity.Services.Email;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Infrastructure.Identity.Options;
using MieleSystem.Infrastructure.Identity.Persistence.Repositories;
using MieleSystem.Infrastructure.Identity.Persistence.Stores;
using MieleSystem.Infrastructure.Identity.Services.Authentication;
using MieleSystem.Infrastructure.Identity.Services.Domain;
using MieleSystem.Infrastructure.Identity.Services.Email;

namespace MieleSystem.Infrastructure.Identity.Injection;

/// <summary>
/// Extensões para registrar os serviços e dependências da camada Identity na injeção de dependência.
/// </summary>
public static class InfrastructureIdentityInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Repositórios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserAuditLogRepository, UserAuditLogRepository>();

        // Read Stores
        services.AddScoped<IUserReadStore, UserReadStore>();
        services.AddScoped<IRefreshTokenReadStore, RefreshTokenReadStore>();
        services.AddScoped<IUserConnectionLogReadStore, UserConnectionLogReadStore>();

        // Serviços de Hashing (appsettings.json / appsettings.{Environment}.json)
        services
            .AddOptions<BCryptOptions>()
            .Bind(configuration.GetSection("Security:BCrypt"))
            .Validate(
                opt => opt.WorkFactor >= 4 && opt.WorkFactor <= 16,
                "Security:BCrypt:WorkFactor deve estar entre 4 e 16."
            )
            .ValidateOnStart();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();

        // Serviços de geração de tokens
        services.AddScoped<ITokenService, TokenService>();

        // Serviços de geração de OTP (appsettings.json / appsettings.{Environment}.json)
        services
            .AddOptions<OtpOptions>()
            .Bind(configuration.GetSection("Security:Otp"))
            .Validate(
                o => o.ExpirationSeconds >= 30 && o.ExpirationSeconds <= 3600,
                "Security:Otp:ExpirationSeconds deve estar entre 30 e 3600."
            )
            .ValidateOnStart();

        services.AddScoped<IOtpService, OtpService>();

        // Authentication context service
        services.AddScoped<IAuthenticationContextService, AuthenticationContextService>();

        // HTTP Context service for authentication
        services.AddScoped<IHttpContextAuthenticationService, HttpContextAuthenticationService>();

        // Serviços de envio de e-mail
        services
            .AddOptions<EmailSenderOptions>()
            .Bind(configuration.GetSection("Email"))
            .Validate(
                opt =>
                    !string.IsNullOrWhiteSpace(opt.FromEmail)
                    && !string.IsNullOrWhiteSpace(opt.FromName)
                    && !string.IsNullOrWhiteSpace(opt.SmtpHost)
                    && opt.SmtpPort > 0
                    && !string.IsNullOrWhiteSpace(opt.SmtpUsername)
                    && !string.IsNullOrWhiteSpace(opt.SmtpPassword),
                "Configuração inválida para Email (seção 'Email')."
            )
            .ValidateOnStart();

        // Serviços de e-mail modularizados
        services.AddScoped<IEmailTemplateRenderer, SimpleEmailTemplateRenderer>();
        services.AddScoped<IEmailValidator, EmailValidator>();
        services.AddScoped<EmailConfigurationValidator>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IEmailLoggingService, EmailLoggingService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IAccountEmailService, AccountEmailService>();

        // Validação das opções do JWT
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Security:Jwt"))
            .Validate(opt =>
            {
                opt.Validate();
                return true;
            })
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtOptions>>().Value);

        // Serviços de hashing de refresh tokens
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();

        return services;
    }
}
