namespace Zeus.Academia.Features.SharedKernel.Foundation.Primitives;

public sealed class Result<TValue> : Result
{
  private readonly TValue? _value;

  private Result(TValue value)
      : base(true, Error.None)
  {
    _value = value;
  }

  private Result(Error error)
      : base(false, error)
  {
  }

  public TValue Value => IsSuccess
      ? _value ?? throw new InvalidOperationException("Successful result must include a value.")
      : throw new InvalidOperationException("Cannot access Value when result is a failure.");

  public static Result<TValue> Success(TValue value)
  {
    ArgumentNullException.ThrowIfNull(value);
    return new Result<TValue>(value);
  }

  public static new Result<TValue> Failure(Error error)
  {
    return new Result<TValue>(error);
  }
}
