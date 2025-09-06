using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MieleSystem.Infrastructure.Common.Persistence;

namespace MieleSystem.Infrastructure.Common.Factory;

/// <summary>
/// Factory usada pelo EF Core em design-time (dotnet-ef) para criar o MieleDbContext.
/// </summary>
public sealed class MieleDbContextFactory : IDesignTimeDbContextFactory<MieleDbContext>
{
    public MieleDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // 1) Resolve um BasePath confiável (tenta Presentation primeiro)
        var basePath = ResolveBasePath();

        // 2) Constrói IConfiguration com appsettings + variáveis de ambiente
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        // 3) Prioriza a env var (compatível com InfrastructureCommonInjection)
        var connectionString =
            Environment.GetEnvironmentVariable("CONNECTION_STRINGS__DEFAULTCONNECTION")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string não encontrada. Defina CONNECTION_STRINGS__DEFAULTCONNECTION ou DefaultConnection nos appsettings."
            );

        // Configuração para compatibilidade de timestamp do Npgsql
        // Permite que DateTime.UtcNow seja salvo em timestamp without time zone
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // 4) Monta o DbContextOptions exatamente como em runtime
        var optionsBuilder = new DbContextOptionsBuilder<MieleDbContext>().UseNpgsql(
            connectionString,
            npgsql =>
            {
                // Mantém as migrations no assembly da Infrastructure
                npgsql.MigrationsAssembly(typeof(MieleDbContext).Assembly.FullName);
            }
        );

        return new MieleDbContext(optionsBuilder.Options);
    }

    private static string ResolveBasePath()
    {
        // Quando o dotnet-ef roda a partir do projeto da Infrastructure, os appsettings
        // costumam estar na Presentation. Abaixo, tentamos alguns caminhos comuns.
        var cwd = Directory.GetCurrentDirectory();

        // 1) Diretório atual (caso appsettings estejam aqui)
        if (File.Exists(Path.Combine(cwd, "appsettings.json")))
            return cwd;

        // 2) Presentation (padrão do seu repo)
        var presentation = Path.GetFullPath(Path.Combine(cwd, "..", "MieleSystem.Presentation"));
        if (File.Exists(Path.Combine(presentation, "appsettings.json")))
            return presentation;

        // 3) Volta um nível (mono-repo com src/)
        var srcPresentation = Path.GetFullPath(
            Path.Combine(cwd, "..", "..", "src", "MieleSystem.Presentation")
        );
        if (File.Exists(Path.Combine(srcPresentation, "appsettings.json")))
            return srcPresentation;

        // 4) Fallback: diretório atual mesmo (permite usar só variáveis de ambiente)
        return cwd;
    }
}
