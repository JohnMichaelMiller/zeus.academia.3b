using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.ListRanks;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ListRanksQueryHandlerTests
{
  [Fact]
  public async Task Handle_ReturnsStableCanonicalOrderWithAccessLevels()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlite(connection)
        .Options;

    await using var context = new SharedKernelDbContext(options);
    await context.Database.EnsureCreatedAsync();

    context.Ranks.Add(ManagedRank.Create(Rank.L));
    context.Ranks.Add(ManagedRank.Create(Rank.P));
    context.Ranks.Add(ManagedRank.Create(Rank.SL));
    await context.SaveChangesAsync();

    var handler = new ListRanksQueryHandler(context);

    var result = await handler.Handle(new ListRanksQuery(), CancellationToken.None);

    Assert.True(result.IsSuccess);

    var rows = result.Value;
    Assert.Equal(3, rows.Count);
    Assert.Collection(
        rows,
        row =>
        {
          Assert.Equal("P", row.Code);
          Assert.Equal("INT", row.AccessLevel);
        },
        row =>
        {
          Assert.Equal("SL", row.Code);
          Assert.Equal("NAT", row.AccessLevel);
        },
        row =>
        {
          Assert.Equal("L", row.Code);
          Assert.Equal("LOC", row.AccessLevel);
        });
  }
}
