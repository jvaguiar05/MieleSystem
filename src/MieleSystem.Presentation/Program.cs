using DotNetEnv;
using MieleSystem.Presentation.Injection;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Services.AddAPI(builder.Configuration);

var app = builder.Build();

app.UseAPI();

app.Run();
