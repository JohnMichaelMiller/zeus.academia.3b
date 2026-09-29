using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class DegreeCodeCatalogTests
{
  [Fact]
  public void TryParseDegree_WithSupportedCode_ReturnsNormalizedDegree()
  {
    var isValid = DegreeCodeCatalog.TryParseDegree(" phd ", out var degree);

    Assert.True(isValid);
    Assert.NotNull(degree);
    Assert.Equal("PHD", degree.Code);
  }

  [Fact]
  public void TryParseDegree_WithUnsupportedCode_ReturnsFalseAndNull()
  {
    var isValid = DegreeCodeCatalog.TryParseDegree("XYZ", out var degree);

    Assert.False(isValid);
    Assert.Null(degree);
  }
}
