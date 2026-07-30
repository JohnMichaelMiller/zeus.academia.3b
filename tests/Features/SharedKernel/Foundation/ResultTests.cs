using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ResultTests
{
  [Fact]
  public void Success_Result_IsSuccessful()
  {
    var result = Result.Success();

    Assert.True(result.IsSuccess);
    Assert.False(result.IsFailure);
  }

  [Fact]
  public void Failure_Result_GuardsNullError()
  {
    Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
  }

  [Fact]
  public void GenericSuccess_Result_ReturnsValue()
  {
    var result = Result<string>.Success("value");

    Assert.True(result.IsSuccess);
    Assert.Equal("value", result.Value);
  }

  [Fact]
  public void GenericFailure_Result_ThrowsWhenValueAccessed()
  {
    var result = Result<string>.Failure(Error.Create("ERR", "Failure"));

    var exception = Assert.Throws<InvalidOperationException>(() => _ = result.Value);

    Assert.Contains("failed result", exception.Message);
  }

  [Fact]
  public void GenericFailure_Result_GuardsNullError()
  {
    Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(null!));
  }
}
