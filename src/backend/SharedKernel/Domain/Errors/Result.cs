namespace Zeus.Academia.SharedKernel.Domain.Errors;

/// <summary>
/// Outcome wrapper for operations that may fail with a domain
/// <see cref="Error"/>. Prefer <see cref="Result{T}"/> when a value is returned.
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

    /// <summary>True when the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>True when the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Error describing the failure. Meaningful only when <see cref="IsFailure"/>.</summary>
    public Error Error { get; }

    /// <summary>Creates a successful, value-less result.</summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>Creates a failed result from an <see cref="Error"/>.</summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Creates a successful result with a value.</summary>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>Creates a failed result with a value type.</summary>
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}
