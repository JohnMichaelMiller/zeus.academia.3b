namespace Zeus.Academia.SharedKernel.Domain.Results;

/// <summary>
/// Represents the outcome of an operation as either success or failure with
/// a typed <see cref="Error"/>. Prefer returning <see cref="Result"/> from
/// command handlers rather than throwing for expected failure paths.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("A successful result cannot carry an error.");
        }
        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("A failed result must carry an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.FromValue(value);
    public static Result<T> Failure<T>(Error error) => Result<T>.FromError(error);
}

/// <summary>
/// A <see cref="Result"/> that carries a value on success.
/// </summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value) : base(true, Error.None)
    {
        _value = value;
    }

    private Result(Error error) : base(false, error)
    {
        _value = default;
    }

    /// <summary>
    /// The success value. Throws if the result is a failure — always check
    /// <see cref="Result.IsSuccess"/> first.
    /// </summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    internal static Result<T> FromValue(T value) => new(value);
    internal static Result<T> FromError(Error error) => new(error);

    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(Error error) => new(error);
}
