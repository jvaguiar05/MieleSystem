namespace MieleSystem.Application.Common.Responses;

/// <summary>
/// Representa o resultado de uma operação (com ou sem valor).
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true);

    public static Result Failure(string error) => new(false, error);

    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}

/// <summary>
/// Representa o resultado de uma operação com valor de retorno.
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value)
        : base(true)
    {
        Value = value;
    }

    private Result(string error)
        : base(false, error)
    {
        Value = default;
    }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(string error) => new(error);

    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
}
