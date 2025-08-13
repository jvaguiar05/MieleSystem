using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MieleSystem.Application.Identity.Services;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Infrastructure.Identity.Email;
using MieleSystem.Infrastructure.Identity.Events.User;
using MieleSystem.Infrastructure.Identity.Options;
using MieleSystem.Infrastructure.Identity.Persistence.Repositories;
using MieleSystem.Infrastructure.Identity.Persistence.Stores;
using MieleSystem.Infrastructure.Identity.Services;

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

        // Serviços de Hashing
        services.Configure<BCryptOptions>(configuration.GetSection("Security:BCrypt"));
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Serviços de geração de tokens
        services.Configure<JwtOptions>(configuration.GetSection("Security:Jwt"));
        services.AddScoped<ITokenService, TokenService>();

        // Serviços de geração de OTP
        services.Configure<OtpOptions>(configuration.GetSection("Security:Otp"));
        services.AddScoped<IOtpService, OtpService>();

        // Serviços de envio de e-mail
        services.Configure<EmailSenderOptions>(configuration.GetSection("Email"));
        services.AddScoped<IAccountEmailService, AccountEmailService>();

        // Handlers de eventos
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(UserRegisteredEventHandler).Assembly)
        );

        return services;
    }
}
