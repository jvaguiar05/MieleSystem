using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MieleSystem.Application.Common.Behaviors;

namespace MieleSystem.Application.Common.Injection;

/// <summary>
/// Contém os métodos de extensão responsáveis por registrar os serviços
/// genéricos da camada Application que são compartilhados entre contextos.
/// </summary>
public static class ApplicationCommonInjection
{
    /// <summary>
    /// Registra os serviços comuns da camada Application (ex: Behaviors, Utils, AutoMapper, etc.).
    /// </summary>
    public static IServiceCollection AddApplicationCommon(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationCommonInjection).Assembly;

        // Behaviors MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper (inclui todos os Profiles dos Bounded Contexts)
        services.AddAutoMapper(assembly);

        return services;
    }
}
