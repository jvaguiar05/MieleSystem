namespace MieleSystem.Application.Common.DTOs;

/// <summary>
/// Representa um resultado genérico de uma operação, com sucesso, erros e payload opcional.
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
}
