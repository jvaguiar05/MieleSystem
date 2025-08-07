using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MieleSystem.Application.Common.Behaviors;

/// <summary>
/// Pipeline Behavior que monitora o tempo de execução de comandos e queries.
/// Registra logs para handlers que ultrapassam o tempo limite definido.
/// </summary>
public class PerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;
    private readonly Stopwatch _timer = new Stopwatch();

    private const int WarningThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default
    )
    {
        _timer.Start();

        var response = await next(ct);

        _timer.Stop();

        var elapsedMs = _timer.ElapsedMilliseconds;

        if (elapsedMs > WarningThresholdMs)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogWarning(
                "🕒 Handler {RequestName} demorou {ElapsedMilliseconds}ms para processar. Dados: {@Request}",
                requestName,
                elapsedMs,
                request
            );
        }

        return response;
    }
}
