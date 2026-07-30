namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public abstract record Result
{
  protected Result(bool isSuccess, Error error)
  {
    if (!isSuccess && error is null)
    {
      throw new ArgumentNullException(nameof(error));
    }

    IsSuccess = isSuccess;
    Error = error;
  }

  public bool IsSuccess { get; }

  public bool IsFailure => !IsSuccess;

  public Error Error { get; }

  public static Result Success() => new SuccessResult();

  public static Result Failure(Error error) => new FailureResult(error);

  private sealed record SuccessResult : Result
  {
    public SuccessResult()
        : base(true, Error.None)
    {
    }
  }

  private sealed record FailureResult : Result
  {
    public FailureResult(Error error)
        : base(false, error)
    {
    }
  }
}
