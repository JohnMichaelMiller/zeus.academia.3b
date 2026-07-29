using Zeus.Academia.Backend.SharedKernel.Common;

namespace Zeus.Academia.Tests.SharedKernel.Common;

public sealed class ResultTests
{
  [Fact]
  public void GenericSuccess_ShouldExposeValue()
  {
    var result = Result<string>.Success("ok");

    Assert.True(result.IsSuccess);
    Assert.Equal("ok", result.Value);
  }

  [Fact]
  public void GenericFailure_ShouldThrowOnValueAccess()
  {
    var result = Result<string>.Failure(Error.Failure("shared.failure", "failed"));

    Assert.True(result.IsFailure);
    Assert.Throws<InvalidOperationException>(() => _ = result.Value);
  }

  [Fact]
  public void GenericSuccess_ShouldRejectNullValue()
  {
    Assert.Throws<ArgumentNullException>(() => Result<string>.Success(null!));
  }
}