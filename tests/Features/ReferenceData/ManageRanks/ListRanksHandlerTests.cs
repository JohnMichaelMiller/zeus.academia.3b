using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class ListRanksHandlerTests
{
  [Fact]
  public async Task Handle_ReturnsStableSortedCodesWithAccessLevelMapping()
  {
    await using var dbContext = CreateInMemoryContext();
    dbContext.Ranks.AddRange(
      new RankRecord { Code = "SL", AccessLevel = "NAT" },
      new RankRecord { Code = "P", AccessLevel = "INT" },
      new RankRecord { Code = "L", AccessLevel = "LOC" });
    await dbContext.SaveChangesAsync();

    var handler = new ListRanksHandler(dbContext);

    var response = await handler.Handle(new ListRanksQuery(), CancellationToken.None);

    Assert.Equal(3, response.Count);
    Assert.Equal(["L", "P", "SL"], response.Select(x => x.Code).ToArray());
    Assert.Equal("LOC", response[0].AccessLevel);
    Assert.Equal("INT", response[1].AccessLevel);
    Assert.Equal("NAT", response[2].AccessLevel);
  }

  private static ManageRanksDbContext CreateInMemoryContext()
  {
    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
      .UseInMemoryDatabase($"ManageRanksListTests-{Guid.NewGuid():N}")
      .Options;

    return new ManageRanksDbContext(options);
  }
}
