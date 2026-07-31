using Microsoft.Data.Sqlite;
using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class ListRanksHandlerTests
{
  [Fact]
  public async Task Handle_WithExistingRanks_ReturnsStableOrderedRanksAndAccessLevels()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await using var context = await ManageRanksTestDbContextFactory.CreateAsync(connection);

    var addHandler = new AddRankHandler(context);
    await addHandler.Handle(new AddRankCommand("SL"), CancellationToken.None);
    await addHandler.Handle(new AddRankCommand("L"), CancellationToken.None);
    await addHandler.Handle(new AddRankCommand("P"), CancellationToken.None);

    var sut = new ListRanksHandler(context);

    var result = await sut.Handle(new ListRanksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);

    var ranks = result.Value.Ranks;
    Assert.Equal(3, ranks.Count);

    Assert.Equal("L", ranks[0].Code);
    Assert.Equal(AccessLevel.LOC, ranks[0].AccessLevel);

    Assert.Equal("P", ranks[1].Code);
    Assert.Equal(AccessLevel.INT, ranks[1].AccessLevel);

    Assert.Equal("SL", ranks[2].Code);
    Assert.Equal(AccessLevel.NAT, ranks[2].AccessLevel);
  }
}
