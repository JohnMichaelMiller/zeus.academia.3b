using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class DegreeCodeCatalogTests
{
  [Fact]
  public void TryParseDegree_WithSupportedCode_ReturnsDegree()
  {
    var isParsed = DegreeCodeCatalog.TryParseDegree(" phd ", out var degree);

    Assert.True(isParsed);
    Assert.NotNull(degree);
    Assert.Equal("PHD", degree.Code);
  }

  [Fact]
  public void TryParseDegree_WithUnsupportedCode_ReturnsNullOutput()
  {
    var isParsed = DegreeCodeCatalog.TryParseDegree("XYZ", out var degree);

    Assert.False(isParsed);
    Assert.Null(degree);
  }
}
