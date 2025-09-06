using MieleSystem.Application.Common.DTOs;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Common.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Converte Result<T> para DTO simples
    /// </summary>
    public static ResultDto<T> ToDto<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? ResultDto<T>.Ok(result.Value!)
            : ResultDto<T>.Fail(
                result.Errors != null && result.Errors.Count > 0
                    ? string.Join("; ", result.Errors.Select(e => e.Message))
                    : "Erro desconhecido."
            );
    }

    /// <summary>
    /// Converte Result para DTO simples
    /// </summary>
    public static ResultDto ToDto(this Result result)
    {
        return result.IsSuccess
            ? ResultDto.Ok()
            : ResultDto.Fail(
                result.Errors != null && result.Errors.Count > 0
                    ? string.Join("; ", result.Errors.Select(e => e.Message))
                    : "Erro desconhecido."
            );
    }

    /// <summary>
    /// Converte Result<T> para objeto de resposta integrado
    /// </summary>
    public static object ToResponse<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? new { success = true, data = result.Value }
            : new { success = false, error = result.Errors.First() };
    }

    /// <summary>
    /// Converte Result para objeto de resposta integrado
    /// </summary>
    public static object ToResponse(this Result result)
    {
        return result.IsSuccess
            ? new { success = true }
            : new { success = false, error = result.Errors.First() };
    }

    /// <summary>
    /// Cria detalhes estruturados de uma exceção para debugging
    /// </summary>
    public static object CreateExceptionDetails(this Exception ex, string operation = "Unknown")
    {
        return new
        {
            message = ex.Message,
            type = ex.GetType().FullName,
            stackTrace = ex.StackTrace,
            innerException = ex.InnerException != null
                ? new
                {
                    message = ex.InnerException.Message,
                    type = ex.InnerException.GetType().FullName,
                    stackTrace = ex.InnerException.StackTrace,
                }
                : null,
            source = ex.Source,
            targetSite = ex.TargetSite?.ToString(),
            data = ex.Data.Count > 0 ? ex.Data : null,
            timestamp = DateTime.UtcNow,
            operation = operation,
        };
    }
}
