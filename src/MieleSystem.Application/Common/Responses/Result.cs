namespace MieleSystem.Application.Common.Responses;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors { get; }

    protected Result(bool isSuccess, IEnumerable<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = (errors ?? Array.Empty<Error>()).ToArray();
    }

    public static Result Success() => new(true);

    public static Result Failure(Error error) => new(false, new[] { error });

    public static Result Failure(IEnumerable<Error> errors) => new(false, errors);

    public Result Then(Func<Result> next) => IsFailure ? this : next();

    public Result<TOut> Then<TOut>(Func<Result<TOut>> next) =>
        IsFailure ? Result<TOut>.Failure(Errors) : next();

    public override string ToString() =>
        IsSuccess ? "Success" : $"Failure: {string.Join(", ", Errors.Select(e => e.Code))}";
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value)
        : base(true) => Value = value;

    private Result(IEnumerable<Error> errors)
        : base(false, errors) { }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => new(new[] { error });

    public static new Result<T> Failure(IEnumerable<Error> errors) => new(errors);

    public Result<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        IsFailure ? Result<TOut>.Failure(Errors) : Result<TOut>.Success(mapper(Value!));

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder) =>
        IsFailure ? Result<TOut>.Failure(Errors) : binder(Value!);
}
