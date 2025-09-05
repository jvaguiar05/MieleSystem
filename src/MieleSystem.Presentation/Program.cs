using DotNetEnv;
using Microsoft.Extensions.Options;
using MieleSystem.Presentation.Injection;
using MieleSystem.Presentation.Utils;

namespace MieleSystem.Presentation;

/// <summary>
/// Ponto de entrada principal da aplicação MieleSystem.
/// Esta classe gerencia a inicialização da aplicação, carregamento de configurações e registro de serviços.
/// </summary>
public class Program
{
    /// <summary>
    /// Ponto de entrada da aplicação.
    /// </summary>
    /// <param name="args">Argumentos da linha de comando</param>
    /// <returns>Código de saída indicando sucesso ou falha</returns>
    public static async Task<int> Main(string[] args)
    {
        try
        {
            // Inicializa a aplicação
            var app = await CreateApplicationAsync(args);

            // Configura e inicia a aplicação
            await ConfigureAndStartApplicationAsync(app);

            return 0; // Sucesso
        }
        catch (Exception ex)
        {
            // Registra falha crítica na inicialização
            Console.WriteLine($"❌ Erro crítico durante a inicialização da aplicação: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return 1; // Falha
        }
    }

    /// <summary>
    /// Cria e configura a instância do WebApplication.
    /// </summary>
    /// <param name="args">Argumentos da linha de comando</param>
    /// <returns>Instância configurada do WebApplication</returns>
    private static async Task<WebApplication> CreateApplicationAsync(string[] args)
    {
        // Cria o construtor da aplicação
        var builder = WebApplication.CreateBuilder(args);

        // Configura o sistema de logging
        ConfigureLogging(builder);

        // Carrega configurações do ambiente
        await LoadEnvironmentConfigurationAsync(builder);

        // Registra serviços da aplicação
        RegisterApplicationServices(builder);

        // Constrói a aplicação
        var app = builder.Build();

        // Configura o pipeline da aplicação
        ConfigureApplicationPipeline(app);

        return app;
    }

    /// <summary>
    /// Configura logging estruturado para a aplicação.
    /// </summary>
    /// <param name="builder">Instância do WebApplicationBuilder</param>
    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        // Configura logging com configurações específicas do ambiente
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // Adiciona logging estruturado em produção
        if (builder.Environment.IsProduction())
        {
            // Em produção, você pode adicionar outros provedores como Serilog, Application Insights, etc.
            builder.Logging.AddEventSourceLogger();
        }
    }

    /// <summary>
    /// Carrega configurações do ambiente a partir de arquivos .env e variáveis de ambiente.
    /// </summary>
    /// <param name="builder">Instância do WebApplicationBuilder</param>
    private static Task LoadEnvironmentConfigurationAsync(WebApplicationBuilder builder)
    {
        try
        {
            // Carrega arquivo .env se existir (ambiente de desenvolvimento)
            if (File.Exists(".env"))
            {
                Env.Load();
                Console.WriteLine("✅ Variáveis de ambiente carregadas do arquivo .env");
            }
            else if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine(
                    "⚠️  Aviso: Arquivo .env não encontrado no ambiente de desenvolvimento"
                );
            }

            // Adiciona variáveis de ambiente como fonte de configuração
            // Isso é CRÍTICO para o funcionamento correto da aplicação
            builder.Configuration.AddEnvironmentVariables();

            Console.WriteLine("✅ Variáveis de ambiente registradas como fonte de configuração");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao carregar configurações do ambiente: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Registra todos os serviços da aplicação com injeção de dependência.
    /// </summary>
    /// <param name="builder">Instância do WebApplicationBuilder</param>
    private static void RegisterApplicationServices(WebApplicationBuilder builder)
    {
        try
        {
            // Registra serviços da API (inclui todas as camadas: Application, Infrastructure, Domain)
            builder.Services.AddAPI(builder.Configuration);

            Console.WriteLine("✅ Serviços da aplicação registrados com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao registrar serviços da aplicação: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Configura o pipeline da aplicação e middleware.
    /// </summary>
    /// <param name="app">Instância do WebApplication</param>
    private static void ConfigureApplicationPipeline(WebApplication app)
    {
        try
        {
            // Configura pipeline de middleware da API
            app.UseAPI();

            Console.WriteLine("✅ Pipeline da aplicação configurado com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao configurar pipeline da aplicação: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Configura e inicia a aplicação com tratamento de erro adequado e logging.
    /// </summary>
    /// <param name="app">Instância do WebApplication</param>
    private static async Task ConfigureAndStartApplicationAsync(WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        try
        {
            // Registra informações de inicialização da aplicação
            LogApplicationStartupInfo(logger, app.Environment);

            // Valida configuração no ambiente de desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                await ValidateConfigurationAsync(logger, app.Services);
            }

            // Registra inicialização bem-sucedida
            logger.LogInformation("🚀 Aplicação MieleSystem iniciada com sucesso");
            logger.LogInformation("🌍 Ambiente: {Environment}", app.Environment.EnvironmentName);
            logger.LogInformation(
                "🔧 Configuração carregada de: {ConfigurationSources}",
                string.Join(", ", GetConfigurationSources(app.Environment))
            );

            // Inicia a aplicação
            await app.RunAsync();
        }
        catch (OptionsValidationException ex)
        {
            logger.LogCritical(
                ex,
                "❌ Falha na validação da configuração! Verifique seu appsettings.json ou arquivo .env. "
                    + "Erro: {ErrorMessage}",
                ex.Message
            );
            throw;
        }
        catch (Exception ex)
        {
            logger.LogCritical(
                ex,
                "❌ Erro crítico durante a inicialização da aplicação: {ErrorMessage}",
                ex.Message
            );
            throw;
        }
    }

    /// <summary>
    /// Registra informações de inicialização da aplicação.
    /// </summary>
    /// <param name="logger">Instância do logger</param>
    /// <param name="environment">Ambiente de hospedagem web</param>
    private static void LogApplicationStartupInfo(ILogger logger, IWebHostEnvironment environment)
    {
        logger.LogInformation("=".PadRight(60, '='));
        logger.LogInformation("🎯 Iniciando Aplicação MieleSystem");
        logger.LogInformation("=".PadRight(60, '='));
        logger.LogInformation(
            "📅 Iniciado em: {StartTime}",
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
        );
        logger.LogInformation("🌍 Ambiente: {Environment}", environment.EnvironmentName);
        logger.LogInformation(
            "📁 Diretório de Conteúdo: {ContentRoot}",
            environment.ContentRootPath
        );
        logger.LogInformation("🌐 Diretório Web: {WebRoot}", environment.WebRootPath);
        logger.LogInformation("=".PadRight(60, '='));
    }

    /// <summary>
    /// Valida configuração da aplicação no ambiente de desenvolvimento.
    /// </summary>
    /// <param name="logger">Instância do logger</param>
    /// <param name="serviceProvider">Provedor de serviços</param>
    private static Task ValidateConfigurationAsync(ILogger logger, IServiceProvider serviceProvider)
    {
        try
        {
            logger.LogInformation("🔍 Validando configuração da aplicação...");

            // Usa o ConfigurationLogger para validar e registrar todas as configurações
            ConfigurationLogger.LogConfigurationVariables(logger, serviceProvider);

            logger.LogInformation("✅ Validação da configuração concluída com sucesso");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Falha na validação da configuração: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Obtém a lista de fontes de configuração baseadas no ambiente.
    /// </summary>
    /// <param name="environment">Ambiente de hospedagem web</param>
    /// <returns>Lista de fontes de configuração</returns>
    private static string[] GetConfigurationSources(IWebHostEnvironment environment)
    {
        var sources = new List<string> { "appsettings.json" };

        if (environment.IsDevelopment())
        {
            sources.Add("appsettings.Development.json");
            sources.Add("arquivo .env");
        }
        else if (environment.IsStaging())
        {
            sources.Add("appsettings.Staging.json");
        }
        else if (environment.IsProduction())
        {
            sources.Add("appsettings.Production.json");
        }

        sources.Add("Variáveis de Ambiente");

        return sources.ToArray();
    }
}
