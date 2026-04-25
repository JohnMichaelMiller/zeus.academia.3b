namespace Zeus.Academia.SharedKernel.Domain.Common;

public sealed class Result<T>
{
    public bool    IsSuccess { get; }
    public T?      Value     { get; }
    public Error?  Error     { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value     = value;
        Error     = error;
    }

    public static Result<T> Success(T value)          => new(true,  value,   null);
    public static Result<T> Failure(Error error)      => new(false, default, error);
    public static Result<T> Failure(string message)   => new(false, default, new Error(message));
}
