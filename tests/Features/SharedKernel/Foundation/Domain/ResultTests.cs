using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class ResultTests
{
  [Fact]
  public void Failure_NonGeneric_GuardsAgainstNullError()
  {
    Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
  }

  [Fact]
  public void Failure_Generic_GuardsAgainstNullError()
  {
    Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(null!));
  }

  [Fact]
  public void Success_Generic_GuardsAgainstNullValue()
  {
    Assert.Throws<InvalidOperationException>(() => Result<string>.Success(null!));
  }

  [Fact]
  public void Value_OnFailure_ThrowsClearException()
  {
    var result = Result<string>.Failure(new Error("ERR", "boom"));

    var exception = Assert.Throws<InvalidOperationException>(() => _ = result.Value);

    Assert.Equal("Cannot access Value when result is a failure.", exception.Message);
  }

  [Fact]
  public void Value_OnSuccess_ReturnsPayload()
  {
    var result = Result<string>.Success("ok");

    Assert.Equal("ok", result.Value);
  }
}
