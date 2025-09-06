using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Domain.Common.Exceptions;

namespace MieleSystem.Presentation.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
)
{
    public async Task Invoke(HttpContext context)
    {
        string correlationId = context.Request.Headers.TryGetValue("X-Correlation-Id", out var cid)
            ? cid.ToString()
            : Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

        try
        {
            context.Response.Headers["X-Correlation-Id"] = correlationId;
            await next(context);
        }
        catch (DomainException ex)
        {
            logger.LogWarning(ex, "Domain error {CorrelationId}", correlationId);
            await WriteProblem(
                context,
                Error.Domain(ex.Code ?? "domain.error", ex.Message, correlationId, ex.Details)
            );
        }
        catch (ValidationException ex)
        {
            var details = new
            {
                errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
            };
            logger.LogInformation(ex, "Validation error {CorrelationId}", correlationId);
            await WriteProblem(
                context,
                Error.Validation("validation.failed", "Dados inválidos.", details, correlationId)
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogInformation(ex, "Unauthorized {CorrelationId}", correlationId);
            await WriteProblem(context, Error.Unauthorized("Não autorizado.", correlationId));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "NotFound {CorrelationId}", correlationId);
            await WriteProblem(
                context,
                Error.NotFound("resource.not_found", ex.Message, correlationId)
            );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation {CorrelationId}", correlationId);
            await WriteProblem(
                context,
                Error.Validation("operation.invalid", ex.Message, correlationId: correlationId)
            );
        }
        catch (ArgumentException ex)
        {
            logger.LogInformation(ex, "Invalid argument {CorrelationId}", correlationId);
            await WriteProblem(
                context,
                Error.Validation("argument.invalid", ex.Message, correlationId: correlationId)
            );
        }
        catch (TimeoutException ex)
        {
            logger.LogWarning(ex, "Timeout {CorrelationId}", correlationId);
            await WriteProblem(
                context,
                Error.Infrastructure("timeout", "Operação excedeu o tempo limite.", correlationId)
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error {CorrelationId}", correlationId);

            var exceptionDetails = ex.CreateExceptionDetails("GlobalExceptionHandler");

            await WriteProblem(
                context,
                Error.Unexpected(
                    "Ocorreu um erro inesperado.",
                    correlationId,
                    details: exceptionDetails
                )
            );
        }
    }

    private static async Task WriteProblem(HttpContext ctx, Error error)
    {
        var problem = new ProblemDetails
        {
            Title = error.Type.ToString(),
            Detail = error.Message,
            Status = error.HttpStatus,
            Type = $"https://httpstatuses.com/{error.HttpStatus}",
            Instance = ctx.Request.Path,
        };

        problem.Extensions["code"] = error.Code;
        problem.Extensions["correlationId"] = error.CorrelationId ?? string.Empty;

        // Se há detalhes de exceção, inclui innerException diretamente
        if (error.Details is not null)
        {
            // Usa reflexão para acessar innerException se existir
            var detailsType = error.Details.GetType();
            var innerExceptionProperty = detailsType.GetProperty("innerException");
            if (innerExceptionProperty != null)
            {
                var innerException = innerExceptionProperty.GetValue(error.Details);
                if (innerException != null)
                {
                    problem.Extensions["innerException"] = innerException;
                }
            }
        }

        if (error.Metadata is not null)
            problem.Extensions["meta"] = error.Metadata;

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = error.HttpStatus;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
