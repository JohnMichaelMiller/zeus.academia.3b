using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ResultTests
{
  [Fact]
  public void Success_HasExpectedState()
  {
    var result = Result.Success();

    Assert.True(result.IsSuccess);
    Assert.False(result.IsFailure);
    Assert.Equal(Error.None, result.Error);
  }

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
  public void GenericSuccess_WithNullValue_ThrowsInvalidOperationException()
  {
    var exception = Assert.Throws<InvalidOperationException>(() => Result<string>.Success(null!));

    Assert.Contains("include a value", exception.Message, StringComparison.OrdinalIgnoreCase);
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

  [Fact]
  public void GenericFailure_CarriesActionableError()
  {
    var error = Error.Create("Academic.Invalid", "Academic is invalid.");
    var failure = Result<int>.Failure(error);

    Assert.True(failure.IsFailure);
    Assert.Equal(error, failure.Error);
  }

  [Fact]
  public void NonGenericFailure_CarriesActionableError()
  {
    var error = Error.Create("Academic.Conflict", "Conflict detected.");

    var failure = Result.Failure(error);

    Assert.True(failure.IsFailure);
    Assert.Equal(error, failure.Error);
  }
}
