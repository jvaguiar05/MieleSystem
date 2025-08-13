using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MieleSystem.Application.Common.Interfaces;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Infrastructure.Common.Persistence;
using MieleSystem.Infrastructure.Common.Services;

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

        // DbContext compartilhado
        services.AddDbContext<MieleDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(MieleDbContext).Assembly.FullName)
            );

            // Opcional: caso você use snake_case no PostgreSQL:
            // options.UseSnakeCaseNamingConvention();
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Serviços transversais
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        return services;
    }
}
