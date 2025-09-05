using DotNetEnv;
using Microsoft.Extensions.Options;
using MieleSystem.Presentation.Injection;
using MieleSystem.Presentation.Utils;

// ====================================================================================
// Ponto de Entrada (Entry Point) da Aplicação MieleSystem
//
// Este arquivo é responsável pela inicialização, configuração de serviços,
// definição do pipeline de requisições HTTP e execução da aplicação web.
// A estrutura utiliza "top-level statements" do C# 10 para uma configuração
// mais limpa e linear, totalmente compatível com ferramentas de design-time
// como o Entity Framework Core.
// ====================================================================================


// ====================================================================================
// FASE 1: Configuração do Application Builder
//
// Inicializa o construtor da aplicação web (`WebApplicationBuilder`), que serve como
// o principal container para registrar serviços, configurações e logging.
// ====================================================================================

var builder = WebApplication.CreateBuilder(args);

// Configura os provedores e níveis de logging para a aplicação.
ConfigureLogging(builder);

// Carrega as variáveis de ambiente de arquivos .env e do sistema operacional.
LoadEnvironmentConfiguration(builder);

// Registra todos os serviços da aplicação no container de injeção de dependência.
RegisterApplicationServices(builder);

// ====================================================================================
// FASE 2: Construção da Aplicação
//
// Cria a instância da aplicação (`WebApplication`) a partir da configuração do builder.
// Neste ponto, o container de injeção de dependência (IServiceProvider) é finalizado
// e todos os serviços registrados estão prontos para serem usados.
// ====================================================================================

var app = builder.Build();

// ====================================================================================
// FASE 3: Configuração do Pipeline de Requisições HTTP
//
// Define a sequência de middleware que irá processar cada requisição HTTP recebida.
// A ordem de registro dos middlewares é crucial para o correto funcionamento da
// aplicação (ex: autenticação deve vir antes da autorização).
// ====================================================================================

ConfigureApplicationPipeline(app);

// ====================================================================================
// FASE 4: Execução da Aplicação
//
// Contém a lógica de inicialização final, como validações e logging de status,
// antes de efetivamente iniciar o servidor web para escutar por requisições.
// O bloco try-catch garante que qualquer falha crítica na inicialização seja
// registrada adequadamente.
// ====================================================================================

// Obtém uma instância do logger para registrar o processo de inicialização.
var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    // Registra informações detalhadas sobre a inicialização da aplicação.
    LogApplicationStartupInfo(logger, app.Environment);

    // Executa validações de configuração apenas em ambiente de desenvolvimento.
    if (app.Environment.IsDevelopment())
    {
        await ValidateConfigurationAsync(logger, app.Services);
    }

    logger.LogInformation("🚀 Aplicação MieleSystem pronta para receber requisições.");

    // Inicia a aplicação, bloqueando a thread para escutar por requisições HTTP.
    await app.RunAsync();
}
catch (OptionsValidationException ex)
{
    // Captura erros específicos de validação de configurações (IOptions).
    logger.LogCritical(
        ex,
        "❌ Falha crítica na validação da configuração. A aplicação será encerrada."
    );
    // Encerra a aplicação com um código de erro.
    Environment.ExitCode = 1;
}
catch (Exception ex)
{
    // Captura qualquer outra exceção crítica durante a inicialização.
    logger.LogCritical(
        ex,
        "❌ Erro crítico não tratado durante a inicialização. A aplicação será encerrada."
    );
    Environment.ExitCode = 1;
}

// ====================================================================================
// MÉTODOS AUXILIARES DE CONFIGURAÇÃO
//
// Funções estáticas que encapsulam a lógica de configuração para manter o
// fluxo principal do programa limpo e organizado.
// ====================================================================================

/// <summary>
/// Configura os provedores de logging para a aplicação.
/// Limpa provedores padrão e adiciona Console, Debug e outros baseados no ambiente.
/// </summary>
/// <param name="builder">A instância do WebApplicationBuilder para configurar o logging.</param>
void ConfigureLogging(WebApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    if (builder.Environment.IsProduction())
    {
        builder.Logging.AddEventSourceLogger();
    }
}

/// <summary>
/// Carrega configurações de ambiente a partir de um arquivo .env e das variáveis de ambiente do sistema.
/// </summary>
/// <param name="builder">A instância do WebApplicationBuilder para adicionar fontes de configuração.</param>
void LoadEnvironmentConfiguration(WebApplicationBuilder builder)
{
    if (File.Exists(".env"))
    {
        Env.Load();
        Console.WriteLine("✅ Variáveis de ambiente carregadas do arquivo .env");
    }
    builder.Configuration.AddEnvironmentVariables();
    Console.WriteLine("✅ Variáveis de ambiente registradas como fonte de configuração");
}

/// <summary>
/// Invoca os métodos de extensão para registrar todos os serviços da aplicação
/// (Application, Infrastructure, Domain) no container de DI.
/// </summary>
/// <param name="builder">A instância do WebApplicationBuilder para registrar serviços.</param>
void RegisterApplicationServices(WebApplicationBuilder builder)
{
    // O método AddAPI encapsula o registro de todas as camadas da aplicação.
    builder.Services.AddAPI(builder.Configuration);
    Console.WriteLine("✅ Serviços da aplicação registrados com sucesso");
}

/// <summary>
/// Invoca os métodos de extensão para configurar o pipeline de middleware da API.
/// </summary>
/// <param name="app">A instância da aplicação WebApplication.</param>
void ConfigureApplicationPipeline(WebApplication app)
{
    // O método UseAPI encapsula a configuração de todos os middlewares (Swagger, Auth, etc.).
    app.UseAPI();
    Console.WriteLine("✅ Pipeline da aplicação configurado com sucesso");
}

/// <summary>
/// Registra no log informações detalhadas sobre o ambiente de inicialização da aplicação.
/// </summary>
/// <param name="logger">A instância do logger a ser utilizada.</param>
/// <param name="environment">Informações sobre o ambiente de hospedagem.</param>
void LogApplicationStartupInfo(ILogger logger, IWebHostEnvironment environment)
{
    logger.LogInformation("=".PadRight(60, '='));
    logger.LogInformation("🎯 Iniciando Aplicação MieleSystem");
    logger.LogInformation("   - Ambiente: {Environment}", environment.EnvironmentName);
    logger.LogInformation("   - Diretório Raiz: {ContentRoot}", environment.ContentRootPath);
    logger.LogInformation("=".PadRight(60, '='));
}

/// <summary>
/// Valida e registra no log as configurações da aplicação para depuração em ambiente de desenvolvimento.
/// </summary>
/// <param name="logger">A instância do logger a ser utilizada.</param>
/// <param name="serviceProvider">O provedor de serviços para resolver as configurações.</param>
async Task ValidateConfigurationAsync(ILogger logger, IServiceProvider serviceProvider)
{
    logger.LogInformation("🔍 Validando configuração da aplicação...");
    ConfigurationLogger.LogConfigurationVariables(logger, serviceProvider);
    logger.LogInformation("✅ Validação da configuração concluída com sucesso");
    await Task.CompletedTask;
}

/// <summary>
/// Declaração da classe parcial 'Program'.
/// Necessária para que as ferramentas de design-time do .NET (como o Entity Framework)
/// possam identificar este arquivo como o ponto de entrada da aplicação ao usar top-level statements.
/// </summary>
public partial class Program { }
