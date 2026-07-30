using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks.Shared;

public sealed class RankCodeMappingTests
{
  [Fact]
  public void GetOrderedRanks_ReturnsCanonicalRankOrder()
  {
    var ranks = RankCodeMapping.GetOrderedRanks();

    Assert.Equal([Rank.P, Rank.SL, Rank.L], ranks);
  }

  [Theory]
  [InlineData(Rank.P, 0)]
  [InlineData(Rank.SL, 1)]
  [InlineData(Rank.L, 2)]
  public void GetSortOrder_ReturnsCanonicalPosition(Rank rank, int expectedOrder)
  {
    var sortOrder = RankCodeMapping.GetSortOrder(rank);

    Assert.Equal(expectedOrder, sortOrder);
  }

  [Fact]
  public void SqlAllowedCodeConstraint_UsesCanonicalAllowedCodes()
  {
    Assert.Equal("[Rank] IN ('P', 'SL', 'L')", RankCodeMapping.SqlAllowedCodeConstraint);
  }
}
