using Zeus.Academia.Features.SharedKernel.Foundation.Primitives;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Primitives;

public sealed class ResultTests
{
  [Fact]
  public void Result_Success_HasExpectedFlags()
  {
    var result = Result.Success();

    Assert.True(result.IsSuccess);
    Assert.False(result.IsFailure);
    Assert.Equal(Error.None, result.Error);
  }

  [Fact]
  public void Result_Failure_RequiresError()
  {
    var error = Error.Create("validation.failed", "Validation failed.");

    var result = Result.Failure(error);

    Assert.True(result.IsFailure);
    Assert.Equal(error, result.Error);
  }

  [Fact]
  public void ResultOfT_Success_ExposesValue()
  {
    var result = Result<string>.Success("ok");

    Assert.Equal("ok", result.Value);
    Assert.True(result.IsSuccess);
  }

  [Fact]
  public void ResultOfT_Failure_ValueAccessThrows()
  {
    var result = Result<string>.Failure(Error.Create("conflict", "conflict occurred"));

    var act = () => _ = result.Value;

    var exception = Assert.Throws<InvalidOperationException>(act);
    Assert.Equal("Cannot access Value when result is a failure.", exception.Message);
  }
}
