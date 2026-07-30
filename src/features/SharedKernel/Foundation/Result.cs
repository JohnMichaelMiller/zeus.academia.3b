namespace Zeus.Academia.Features.SharedKernel.Foundation;

public sealed class Result
{
  private Result(bool isSuccess, Error error)
  {
    if (isSuccess && error != Error.None)
    {
      throw new InvalidOperationException("Successful results cannot carry an error.");
    }

    if (!isSuccess && error == Error.None)
    {
      throw new InvalidOperationException("Failed results must carry a non-empty error.");
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
      throw new ArgumentException("Failure results require a non-empty error.", nameof(error));
    }

    return new Result(false, error);
  }
}
