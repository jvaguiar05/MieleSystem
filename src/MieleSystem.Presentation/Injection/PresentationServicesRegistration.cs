using Microsoft.OpenApi.Models;
using MieleSystem.Application.Common.Injection;
using MieleSystem.Infrastructure.Common.Injection;
using MieleSystem.Presentation.Middlewares;

namespace MieleSystem.Presentation.Injection;

public static class PresentationServicesRegistration
{
    public static IServiceCollection AddAPI(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );
        });
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "MieleSystem.API", Version = "v1" });
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                }
            );
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        Array.Empty<string>()
                    },
                }
            );
        });

        services.AddApplicationCommon();
        services.AddInfrastructureCommon(configuration);

        services.AddTokenServices(configuration);

        var assembliesToScan = AppDomain
            .CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith("MieleSystem"))
            .ToArray();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembliesToScan));

        return services;
    }

    public static IApplicationBuilder UseAPI(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<AuthenticationContextMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MieleSystem.API v1");
            options.RoutePrefix = string.Empty;
        });
        app.UseCors("CorsPolicy");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}
