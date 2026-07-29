namespace Zeus.Academia.Features.SharedKernel.Foundation.Common;

public class Result
{
  protected Result(bool isSuccess, Error error)
  {
    if (isSuccess && error != Error.None)
    {
      throw new ArgumentException("A successful result cannot carry an error.", nameof(error));
    }

    if (!isSuccess && error == Error.None)
    {
      throw new ArgumentException("A failed result must carry an error.", nameof(error));
    }

    IsSuccess = isSuccess;
    Error = error;
  }

  public bool IsSuccess { get; }

  public bool IsFailure => !IsSuccess;

  public Error Error { get; }

  public static Result Success() => new(true, Error.None);

  public static Result Failure(Error error) => new(false, error);
}
