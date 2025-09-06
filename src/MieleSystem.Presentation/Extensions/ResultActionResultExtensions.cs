using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Presentation.Extensions;

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

    private static IActionResult CreateErrorResponse(Error error) =>
        new ObjectResult(
            new
            {
                success = false,
                error = new
                {
                    code = error.Code,
                    message = error.Message,
                    type = error.Type.ToString(),
                    httpStatus = error.HttpStatus,
                    correlationId = error.CorrelationId,
                    details = error.Details,
                    metadata = error.Metadata,
                },
            }
        )
        {
            StatusCode = error.HttpStatus,
        };
}
