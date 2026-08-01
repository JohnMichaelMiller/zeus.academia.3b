using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class AddRankHandlerTests
{
  [Fact]
  public async Task Handle_WhenCodeAllowed_PersistsRankWithDerivedAccessLevel()
  {
    await using var dbContext = CreateInMemoryContext();
    var handler = new AddRankHandler(dbContext);

    var response = await handler.Handle(new AddRankCommand("SL"), CancellationToken.None);

    Assert.Equal("SL", response.Code);
    Assert.Equal("NAT", response.AccessLevel);

    var persisted = await dbContext.Ranks.SingleAsync(x => x.Code == "SL");
    Assert.Equal("NAT", persisted.AccessLevel);
  }

  [Fact]
  public async Task Handle_WhenDuplicateCodeExists_ThrowsRankConflictException()
  {
    await using var dbContext = CreateInMemoryContext();
    dbContext.Ranks.Add(new RankRecord { Code = "P", AccessLevel = "INT" });
    await dbContext.SaveChangesAsync();

    var handler = new AddRankHandler(dbContext);

    var exception = await Assert.ThrowsAsync<RankConflictException>(async () =>
      await handler.Handle(new AddRankCommand("P"), CancellationToken.None));

    Assert.Contains("already exists", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  private static ManageRanksDbContext CreateInMemoryContext()
  {
    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
      .UseInMemoryDatabase($"ManageRanksTests-{Guid.NewGuid():N}")
      .Options;

    return new ManageRanksDbContext(options);
  }
}
