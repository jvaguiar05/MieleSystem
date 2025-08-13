namespace MieleSystem.Application.Common.DTOs;

/// <summary>
/// Representa um resultado genérico de uma operação.
/// </summary>
public class ResultDto<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static ResultDto<T> Ok(T data, string? message = null) =>
        new()
        {
            Success = true,
            Data = data,
            Message = message,
        };

    public static ResultDto<T> Fail(string message) => new() { Success = false, Message = message };

    public static ResultDto<T> Fail(IEnumerable<string> errors) =>
        new() { Success = false, Message = string.Join("; ", errors) };
}

/// <summary>
/// Versão não genérica para operações sem payload.
/// </summary>
public class ResultDto
{
    public bool Success { get; init; }
    public string? Message { get; init; }

    public static ResultDto Ok(string? message = null) =>
        new() { Success = true, Message = message };

    public static ResultDto Fail(string message) => new() { Success = false, Message = message };

    public static ResultDto Fail(IEnumerable<string> errors) =>
        new() { Success = false, Message = string.Join("; ", errors) };
}
