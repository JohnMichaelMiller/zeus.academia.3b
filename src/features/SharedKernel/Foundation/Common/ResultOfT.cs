namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public sealed record Result<T> : Result
{
  private readonly T? _value;

  private Result(bool isSuccess, Error error, T? value)
      : base(isSuccess, error)
  {
    _value = value;
  }

  public T Value => IsSuccess
      ? _value!
      : throw new InvalidOperationException("Cannot access Value for a failed result.");

  public static Result<T> Success(T value)
  {
    if (value is null)
    {
      throw new ArgumentNullException(nameof(value));
    }

    return new Result<T>(true, Error.None, value);
  }

  public static new Result<T> Failure(Error error) => new(false, error, default);
}
