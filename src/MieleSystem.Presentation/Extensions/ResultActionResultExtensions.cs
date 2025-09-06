using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Presentation.Extensions;

public class ErrorResponse
{
    public bool Success { get; set; }
    public ErrorInfo Error { get; set; } = null!;
}

public class ErrorInfo
{
    public string Code { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int HttpStatus { get; set; }
    public string? CorrelationId { get; set; }
    public IReadOnlyDictionary<string, object?>? Metadata { get; set; }
    public object? InnerException { get; set; }
}

public static class ResultActionResultExtensions
{
    /// <summary>
    /// Converte Result para ActionResult com resposta integrada
    /// </summary>
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess
            ? new OkObjectResult(new { success = true })
            : CreateErrorResponse(result.Errors.First());

    /// <summary>
    /// Converte Result<T> para ActionResult com resposta integrada
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? new OkObjectResult(new { success = true, data = result.Value })
            : CreateErrorResponse(result.Errors.First());

    /// <summary>
    /// Converte Result para ActionResult com resposta simples (apenas dados)
    /// </summary>
    public static IActionResult ToSimpleActionResult(this Result result) =>
        result.IsSuccess ? new OkResult() : CreateErrorResponse(result.Errors.First());

    /// <summary>
    /// Converte Result<T> para ActionResult com resposta simples (apenas dados)
    /// </summary>
    public static IActionResult ToSimpleActionResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? new OkObjectResult(result.Value)
            : CreateErrorResponse(result.Errors.First());

    private static IActionResult CreateErrorResponse(Error error)
    {
        var errorResponse = new ErrorResponse
        {
            Success = false,
            Error = new ErrorInfo
            {
                Code = error.Code,
                Message = error.Message,
                Type = error.Type.ToString(),
                HttpStatus = error.HttpStatus,
                CorrelationId = error.CorrelationId,
                Metadata = error.Metadata,
            },
        };

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
                    errorResponse.Error.InnerException = innerException;
                }
            }
        }

        return new ObjectResult(errorResponse) { StatusCode = error.HttpStatus };
    }
}
