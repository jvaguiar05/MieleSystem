using Microsoft.AspNetCore.Http;

namespace MieleSystem.Application.Common.Responses;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Domain,
    Infrastructure,
    Unexpected,
}

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    int HttpStatus,
    string? CorrelationId = null,
    object? Details = null,
    IReadOnlyDictionary<string, object?>? Metadata = null
)
{
    public static Error Unexpected(
        string message,
        string? correlationId = null,
        object? details = null
    ) =>
        new(
            "error.unexpected",
            message,
            ErrorType.Unexpected,
            StatusCodes.Status500InternalServerError,
            correlationId,
            details
        );

    public static Error Validation(
        string code,
        string message,
        object? details = null,
        string? correlationId = null
    ) =>
        new(
            code,
            message,
            ErrorType.Validation,
            StatusCodes.Status400BadRequest,
            correlationId,
            details
        );

    public static Error NotFound(string code, string message, string? correlationId = null) =>
        new(code, message, ErrorType.NotFound, StatusCodes.Status404NotFound, correlationId);

    public static Error Unauthorized(string message, string? correlationId = null) =>
        new(
            "auth.unauthorized",
            message,
            ErrorType.Unauthorized,
            StatusCodes.Status401Unauthorized,
            correlationId
        );

    public static Error Forbidden(string message, string? correlationId = null) =>
        new(
            "auth.forbidden",
            message,
            ErrorType.Forbidden,
            StatusCodes.Status403Forbidden,
            correlationId
        );

    public static Error OtpRequired(
        string message,
        string? correlationId = null,
        object? details = null
    ) =>
        new(
            "auth.otp_required",
            message,
            ErrorType.Validation,
            StatusCodes.Status428PreconditionRequired,
            correlationId,
            details
        );

    public static Error Conflict(string code, string message, string? correlationId = null) =>
        new(code, message, ErrorType.Conflict, StatusCodes.Status409Conflict, correlationId);

    public static Error Domain(
        string code,
        string message,
        string? correlationId = null,
        object? details = null
    ) =>
        new(
            code,
            message,
            ErrorType.Domain,
            StatusCodes.Status422UnprocessableEntity,
            correlationId,
            details
        );

    public static Error Infrastructure(
        string code,
        string message,
        string? correlationId = null,
        object? details = null
    ) =>
        new(
            code,
            message,
            ErrorType.Infrastructure,
            StatusCodes.Status500InternalServerError,
            correlationId,
            details
        );
}
