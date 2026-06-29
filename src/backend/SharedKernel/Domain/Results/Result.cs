namespace Zeus.Academia.SharedKernel.Domain.Results;

/// <summary>
/// Discriminated union representing the outcome of an operation.
/// Use <see cref="Success"/> for happy-path results and <see cref="Failure"/> for error outcomes.
/// </summary>
/// <typeparam name="T">The value type on the success path.</typeparam>
public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>True when the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>True when the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The result value; non-null only when <see cref="IsSuccess"/> is true.</summary>
    public T? Value { get; }

    /// <summary>The error detail; non-null only when <see cref="IsFailure"/> is true.</summary>
    public Error? Error { get; }

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>Creates a failure result carrying <paramref name="error"/>.</summary>
    public static Result<T> Failure(Error error) => new(false, default, error);
}
