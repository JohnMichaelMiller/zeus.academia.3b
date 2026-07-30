namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed class Result<TValue> : Result
{
  private readonly TValue? _value;

  private Result(TValue value)
      : base(true, Error.None)
  {
    if (value is null)
    {
      throw new InvalidOperationException("Successful generic results must include a value.");
    }

    _value = value;
  }

  private Result(Error error)
      : base(false, error)
  {
  }

  public TValue Value => IsSuccess
      ? _value ?? throw new InvalidOperationException("Successful generic results must include a value.")
      : throw new InvalidOperationException("Cannot access Value for a failed result.");

  public static Result<TValue> Success(TValue value) => new(value);

  public static new Result<TValue> Failure(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);
    return new Result<TValue>(error);
  }
}
