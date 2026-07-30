namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

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

  public static Result Failure(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);
    return new Result(false, error);
  }
}

public sealed class Result<TValue> : Result
{
  private readonly TValue? _value;

  private Result(TValue value)
      : base(true, Error.None)
  {
    if (value is null)
    {
      throw new InvalidOperationException("A successful result must include a non-null value.");
    }

    _value = value;
  }

  private Result(Error error)
      : base(false, error)
  {
  }

  public TValue Value => IsSuccess
      ? _value ?? throw new InvalidOperationException("A successful result must include a value.")
      : throw new InvalidOperationException("Cannot access Value when result is a failure.");

  public static Result<TValue> Success(TValue value) => new(value);

  public static new Result<TValue> Failure(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);
    return new Result<TValue>(error);
  }
}
