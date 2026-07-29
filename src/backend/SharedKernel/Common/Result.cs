namespace Zeus.Academia.Backend.SharedKernel.Common;

public class Result
{
  protected Result(bool isSuccess, Error error)
  {
    ArgumentNullException.ThrowIfNull(error);

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

  public static Result Failure(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);

    if (error == Error.None)
    {
      throw new ArgumentException("A failed result requires a non-empty error.", nameof(error));
    }

    return new(false, error);
  }
}
