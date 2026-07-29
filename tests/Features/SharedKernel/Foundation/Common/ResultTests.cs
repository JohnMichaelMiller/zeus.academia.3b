using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Common;

public sealed class ResultTests
{
  [Fact]
  public void Success_ShouldReturnResultWithoutError()
  {
    var result = Result.Success();

    Assert.True(result.IsSuccess);
    Assert.Equal(Error.None, result.Error);
  }

  [Fact]
  public void Failure_ShouldCarryError()
  {
    var error = Error.Validation("validation.empnr", "Employee number is invalid.");
    var result = Result.Failure(error);

    Assert.True(result.IsFailure);
    Assert.Equal(error, result.Error);
  }

  [Fact]
  public void GenericSuccess_ShouldCarryValue()
  {
    var result = Result<int>.Success(42);

    Assert.True(result.IsSuccess);
    Assert.Equal(42, result.Value);
  }

  [Fact]
  public void GenericFailure_ShouldCarryError()
  {
    var error = Error.Conflict("academic.empnr.duplicate", "Employee number already exists.");
    var result = Result<string>.Failure(error);

    Assert.True(result.IsFailure);
    Assert.Equal(error, result.Error);
  }
}
