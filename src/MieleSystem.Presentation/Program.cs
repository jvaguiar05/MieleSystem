using DotNetEnv;
using Microsoft.Extensions.Configuration;
using MieleSystem.Presentation.Injection;

var builder = WebApplication.CreateBuilder(args);

// 1) Carrega .env -> vira variáveis de ambiente do processo
Env.Load();

// 2) Garante provider de env vars (normalmente já vem, mas deixamos explícito)
builder.Configuration.AddEnvironmentVariables();

// 3) (Opcional, mas útil) Se existir uma connection string vinda do .env,
//    sobrescreve a DefaultConnection antes de expandir placeholders
var envConn =
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (!string.IsNullOrWhiteSpace(envConn))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = envConn;
}

// 4) Expande placeholders ${VAR} presentes no appsettings.json
ExpandEnvPlaceholders(builder.Configuration);

builder.Services.AddAPI(builder.Configuration);

var app = builder.Build();

app.UseAPI();

app.Run();

static void ExpandEnvPlaceholders(IConfiguration config)
{
    // Percorre todas as chaves já carregadas e aplica Environment.ExpandEnvironmentVariables
    foreach (var kvp in config.AsEnumerable())
    {
        var key = kvp.Key;
        var value = kvp.Value;
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            continue;

        // Expande apenas se contiver ${...}
        if (value.Contains("${"))
        {
            var expanded = Environment.ExpandEnvironmentVariables(value);
            if (!ReferenceEquals(expanded, value))
            {
                // IConfiguration gerenciado por ConfigurationManager permite set
                config[key] = expanded;
            }
        }
    }
}
