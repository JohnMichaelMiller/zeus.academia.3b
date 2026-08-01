namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public class Result
{
  protected Result(bool isSuccess, Error error)
  {
    if (isSuccess && error != Error.None)
    {
      throw new InvalidOperationException("Successful results must use Error.None.");
    }

    if (!isSuccess && error == Error.None)
    {
      throw new InvalidOperationException("Failure results must include a non-empty error.");
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
