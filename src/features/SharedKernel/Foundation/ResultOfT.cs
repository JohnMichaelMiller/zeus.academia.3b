namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class Result<TValue>
    where TValue : notnull
{
  private readonly TValue? _value;

  private Result(TValue value)
  {
    ArgumentNullException.ThrowIfNull(value);

    IsSuccess = true;
    _value = value;
    Error = Error.None;
  }

  private Result(Error error)
  {
    IsSuccess = false;
    Error = error;
  }

  public bool IsSuccess { get; }

  public bool IsFailure => !IsSuccess;

  public Error Error { get; }

  public TValue Value =>
      IsSuccess
          ? _value ?? throw new InvalidOperationException("Successful results must include a value.")
          : throw new InvalidOperationException("Cannot access Value when the result is a failure.");

  public static Result<TValue> Success(TValue value) => new(value);

  public static Result<TValue> Failure(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);

    if (error == Error.None)
    {
      throw new ArgumentException("Failure results require a non-empty error.", nameof(error));
    }

    return new Result<TValue>(error);
  }
}
