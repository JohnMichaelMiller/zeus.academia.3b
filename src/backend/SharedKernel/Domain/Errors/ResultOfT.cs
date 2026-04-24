namespace Zeus.Academia.SharedKernel.Domain.Errors;

/// <summary>
/// Outcome wrapper returning a value on success or a domain
/// <see cref="Error"/> on failure.
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

    /// <summary>The success value. Throws if the result is a failure.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    /// <summary>Creates a successful result with a value.</summary>
    public static Result<T> Success(T value) => new(value);

    /// <summary>Creates a failed result.</summary>
    public static new Result<T> Failure(Error error) => new(error);

    /// <summary>Implicit conversion from value to a successful result.</summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>Implicit conversion from error to a failed result.</summary>
    public static implicit operator Result<T>(Error error) => Failure(error);
}
