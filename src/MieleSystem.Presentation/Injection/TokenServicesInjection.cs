using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Presentation.Injection;

public static class TokenServicesInjection
{
    public static IServiceCollection AddTokenServices(this IServiceCollection services)
    {
        var jwtOptions = new JwtOptions
        {
            Secret =
                Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("JWT_SECRET não foi configurado."),
            Issuer =
                Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? throw new InvalidOperationException("JWT_ISSUER não foi configurado."),
            Audience =
                Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                ?? throw new InvalidOperationException("JWT_AUDIENCE não foi configurado."),
            AccessTokenExpiration = TimeSpan.FromMinutes(
                double.TryParse(
                    Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION"),
                    out var expiration
                )
                    ? expiration
                    : throw new InvalidOperationException("JWT_ACCESS_TOKEN_EXPIRATION é inválido.")
            ),
        };

        jwtOptions.Validate();
        services.AddSingleton(jwtOptions);

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
