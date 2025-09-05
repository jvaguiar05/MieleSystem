// MieleSystem.Presentation.Injection/TokenServicesInjection.cs

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Presentation.Injection;

public static class TokenServicesInjection
{
    public static IServiceCollection AddTokenServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // 1. Obtém a seção de configuração usando a constante da classe de opções.
        var jwtOptionsSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = jwtOptionsSection.Get<JwtOptions>();

        // Validação para garantir que as configurações essenciais existem no appsettings.json
        if (jwtOptions is null || string.IsNullOrEmpty(jwtOptions.Secret))
        {
            throw new InvalidOperationException(
                $"A seção de configuração '{JwtOptions.SectionName}' não foi encontrada ou a chave 'Secret' está vazia."
            );
        }

        // 2. Registra as opções no container de DI para que possam ser injetadas em outros lugares (padrão IOptions).
        services.Configure<JwtOptions>(jwtOptionsSection);
        
        // 3. Configura a autenticação JWT.
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Secret)
                    ),
                    ClockSkew = TimeSpan.Zero,
                };
            });

        // 4. Configura as políticas de autorização (nenhuma mudança necessária aqui).
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });

            options.AddPolicy("AdminOrEditor", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin", "Editor");
            });

            options.AddPolicy("AdminEditorOrViewer", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin", "Editor", "Viewer");
            });
            
            options.AddPolicy("NotSuspended", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context => 
                    !context.User.IsInRole("Suspended")
                );
            });
        });

        return services;
    }
}