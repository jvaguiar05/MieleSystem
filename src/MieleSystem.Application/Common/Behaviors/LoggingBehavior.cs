using MediatR;
using Microsoft.Extensions.Logging;

namespace MieleSystem.Application.Common.Behaviors;

/// <summary>
/// Pipeline Behavior que registra no log a execução de qualquer comando ou query
/// (entrada e saída, de forma opcional).
/// </summary>
public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default
    )
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "➡️ [MediatR] Handling {RequestName}: {@Request}",
            requestName,
            request
        );

        var response = await next(ct);

        _logger.LogInformation("✅ [MediatR] Handled {RequestName}", requestName);

        return response;
    }
}
