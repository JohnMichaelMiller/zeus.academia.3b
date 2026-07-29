namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public readonly record struct Result<T>
{
    private Result(T value)
    {
        Value = value;
        Error = Error.None;
        IsSuccess = true;
    }

    private Result(Error error)
    {
        Value = default;
        Error = error;
        IsSuccess = false;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public Error Error { get; }

    public static Result<T> Success(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new Result<T>(value);
    }

    public static Result<T> Failure(Error error)
    {
        if (error.IsNone)
        {
            throw new ArgumentException("A failure result requires a real error.", nameof(error));
        }

        return new Result<T>(error);
    }
}