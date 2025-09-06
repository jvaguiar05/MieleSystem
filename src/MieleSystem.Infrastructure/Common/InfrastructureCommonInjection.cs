using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MieleSystem.Application.Common.Interfaces;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Infrastructure.Common.Persistence;
using MieleSystem.Infrastructure.Common.Services;
using MieleSystem.Infrastructure.Identity.Injection;

namespace MieleSystem.Infrastructure.Common;

/// <summary>
/// Injeção de dependências para Infrastructure/Common.
/// </summary>
public static class InfrastructureCommonInjection
{
    public static IServiceCollection AddInfrastructureCommon(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHttpContextAccessor();

        // Obtém a string de conexão do ambiente ou do arquivo de configuração
        var connectionString =
            Environment.GetEnvironmentVariable("CONNECTION_STRINGS__DEFAULTCONNECTION")
            ?? configuration.GetConnectionString("DefaultConnection");

        // Configuração para compatibilidade de timestamp do Npgsql
        // Permite que DateTime.UtcNow seja salvo em timestamp without time zone
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // DbContext compartilhado
        services.AddDbContext<MieleDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(MieleDbContext).Assembly.FullName)
            );
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Serviços transversais
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        services.AddIdentityInfrastructure(configuration);

        return services;
    }
}
