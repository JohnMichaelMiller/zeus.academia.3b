using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ResultTests
{
  [Fact]
  public void Success_BuildsSuccessfulResultWithNoError()
  {
    var result = Result.Success();

    Assert.True(result.IsSuccess);
    Assert.False(result.IsFailure);
    Assert.Equal(Error.None, result.Error);
  }

  [Fact]
  public void Failure_BuildsFailedResultWithError()
  {
    var error = Error.Validation("validation.empNr", "Employee number is invalid.");
    var result = Result.Failure(error);

    Assert.True(result.IsFailure);
    Assert.Equal(error, result.Error);
  }

  [Fact]
  public void SuccessOfT_BuildsTypedSuccessfulResult()
  {
    var result = Result<string>.Success("ok");

    Assert.True(result.IsSuccess);
    Assert.Equal("ok", result.Value);
  }
}
