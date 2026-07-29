namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed class Result<TValue> : Result
{
  private Result(TValue value)
    : base(true, Error.None)
  {
    Value = value;
  }

  private Result(Error error)
    : base(false, error)
  {
    Value = default;
  }

  public TValue? Value { get; }

  public static Result<TValue> Success(TValue value)
  {
    ArgumentNullException.ThrowIfNull(value);
    return new(value);
  }

  public static new Result<TValue> Failure(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);
    return new(error);
  }
}
