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
    Value = default!;
  }

  public TValue Value { get; }

  public static Result<TValue> Success(TValue value)
  {
    if (value is null)
    {
      throw new ArgumentNullException(nameof(value));
    }

    return new(value);
  }

  public static new Result<TValue> Failure(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);

    if (error == Error.None)
    {
      throw new ArgumentException("A failed result requires a non-empty error.", nameof(error));
    }

    return new(error);
  }
}
