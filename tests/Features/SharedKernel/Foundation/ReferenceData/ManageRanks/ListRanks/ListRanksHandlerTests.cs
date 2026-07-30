using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks.ListRanks;

public sealed class ListRanksHandlerTests
{
  [Fact]
  public async Task Handle_WithPersistedRanks_ReturnsStableRankOrderAndAccessLevels()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    await using var context = await CreateContextAsync(connection);
    context.Ranks.AddRange(
      ManagedRank.Create(Rank.SL),
      ManagedRank.Create(Rank.L),
      ManagedRank.Create(Rank.P));
    await context.SaveChangesAsync();

    var handler = new ListRanksHandler(context);

    var result = await handler.Handle(new ListRanksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(["P", "SL", "L"], result.Value.Ranks.Select(x => x.Code).ToArray());
    Assert.Equal([AccessLevel.INT, AccessLevel.NAT, AccessLevel.LOC], result.Value.Ranks.Select(x => x.AccessLevel).ToArray());
  }

  private static async Task<ManageRanksDbContext> CreateContextAsync(SqliteConnection connection)
  {
    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
        .UseSqlite(connection)
        .Options;

    var context = new ManageRanksDbContext(options);
    await context.Database.EnsureCreatedAsync();
    return context;
  }
}
