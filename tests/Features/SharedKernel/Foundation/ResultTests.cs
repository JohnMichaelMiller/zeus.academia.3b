using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ResultTests
{
  [Fact]
  public void Failure_WithNullError_ThrowsArgumentNullException()
  {
    Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
    Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(null!));
  }

  [Fact]
  public void Failure_WithErrorNone_ThrowsInvalidOperationException()
  {
    var exception = Assert.Throws<InvalidOperationException>(() => Result.Failure(Error.None));

    Assert.Contains("non-empty error", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void GenericFailure_ValueAccess_ThrowsInvalidOperationException()
  {
    var failure = Result<string>.Failure(Error.Create("Academic.NotFound", "Academic does not exist."));

    Assert.True(failure.IsFailure);
    Assert.Throws<InvalidOperationException>(() => _ = failure.Value);
  }

  [Fact]
  public void GenericSuccess_ValueAccess_ReturnsValue()
  {
    var success = Result<string>.Success("OK");

    Assert.True(success.IsSuccess);
    Assert.Equal("OK", success.Value);
  }
}
