using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Presentation.Injection;

public static class TokenServicesInjection
{
    public static IServiceCollection AddTokenServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var jwtOptions = serviceProvider.GetRequiredService<JwtOptions>();

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

        var authorization = services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "Admin Policy",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                }
            );

            options.AddPolicy(
                "User Policy",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("User", "Admin");
                }
            );

            options.AddPolicy(
                "Guest Policy",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Guest", "User", "Admin");
                }
            );
        });

        return services;
    }
}
