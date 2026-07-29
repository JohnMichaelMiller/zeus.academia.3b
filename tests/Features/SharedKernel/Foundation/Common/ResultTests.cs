using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Common;

public sealed class ResultTests
{
  [Fact]
  public void Success_ShouldProduceNonGenericSuccessfulResult()
  {
    var result = Result.Success();

    Assert.True(result.IsSuccess);
    Assert.False(result.IsFailure);
    Assert.Equal(Error.None, result.Error);
  }

  [Fact]
  public void Failure_ShouldProduceNonGenericFailedResult()
  {
    var error = Error.Conflict("Academic.Duplicate", "Academic already exists.");
    var result = Result.Failure(error);

    Assert.False(result.IsSuccess);
    Assert.True(result.IsFailure);
    Assert.Equal(error, result.Error);
  }

  [Fact]
  public void GenericSuccess_ShouldExposeValue()
  {
    var result = Result<string>.Success("ok");

    Assert.True(result.IsSuccess);
    Assert.Equal("ok", result.Value);
  }

  [Fact]
  public void GenericFailure_ValueAccessShouldThrow()
  {
    var result = Result<string>.Failure(Error.Validation("Academic.Invalid", "Invalid payload."));

    var exception = Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    Assert.Equal("Cannot access Value when result is a failure.", exception.Message);
  }
}