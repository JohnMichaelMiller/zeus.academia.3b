using Zeus.Academia.Features.SharedKernel.Foundation;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ResultTests
{
  [Fact]
  public void Success_CreatesSuccessfulNonGenericResult()
  {
    var result = Result.Success();

    Assert.True(result.IsSuccess);
    Assert.False(result.IsFailure);
    Assert.Equal(Error.None, result.Error);
  }

  [Fact]
  public void Failure_WithNullError_ThrowsArgumentNullException()
  {
    var action = () => Result.Failure(null!);

    Assert.Throws<ArgumentNullException>(action);
  }

  [Fact]
  public void GenericSuccess_ExposesValue()
  {
    var result = Result<string>.Success("ready");

    Assert.True(result.IsSuccess);
    Assert.Equal("ready", result.Value);
  }

  [Fact]
  public void GenericFailure_WithNullError_ThrowsArgumentNullException()
  {
    var action = () => Result<string>.Failure(null!);

    Assert.Throws<ArgumentNullException>(action);
  }

  [Fact]
  public void Value_OnFailure_ThrowsInvalidOperationException()
  {
    var result = Result<string>.Failure(Error.Create("rank.invalid", "Invalid rank."));

    var exception = Assert.Throws<InvalidOperationException>(() => _ = result.Value);

    Assert.Equal("Cannot access Value when the result is a failure.", exception.Message);
  }
}
