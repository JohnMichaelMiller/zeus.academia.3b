using Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class ListRanksHandlerTests
{
  [Fact]
  public async Task Handle_ReturnsStoredRanksInCanonicalOrderWithAccessLevels()
  {
    await using var testContext = await ManageRanksSqliteTestContext.CreateAsync();

    await using (var seedContext = testContext.CreateDbContext())
    {
      seedContext.RankReferences.Add(RankReference.Create("L"));
      seedContext.RankReferences.Add(RankReference.Create("P"));
      seedContext.RankReferences.Add(RankReference.Create("SL"));
      await seedContext.SaveChangesAsync();
    }

    await using var dbContext = testContext.CreateDbContext();
    var handler = new ListRanksHandler(dbContext);

    var result = await handler.Handle(new ListRanksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Collection(
        result.Value,
        rank =>
        {
          Assert.Equal("P", rank.Code);
          Assert.Equal(AccessLevel.INT, rank.AccessLevel);
        },
        rank =>
        {
          Assert.Equal("SL", rank.Code);
          Assert.Equal(AccessLevel.NAT, rank.AccessLevel);
        },
        rank =>
        {
          Assert.Equal("L", rank.Code);
          Assert.Equal(AccessLevel.LOC, rank.AccessLevel);
        });
  }
}
