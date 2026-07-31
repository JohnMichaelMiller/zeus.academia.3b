using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AddRankCommandHandlerTests
{
  [Fact]
  public async Task Handle_WithValidCode_PersistsRankWithDerivedAccessLevel()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlite(connection)
        .Options;

    await using var context = new SharedKernelDbContext(options);
    await context.Database.EnsureCreatedAsync();

    var handler = new AddRankCommandHandler(context);

    var result = await handler.Handle(new AddRankCommand("SL"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("SL", result.Value.Code);
    Assert.Equal("NAT", result.Value.AccessLevel);

    var persisted = await context.Ranks.SingleAsync(x => x.Code == "SL");
    Assert.Equal("NAT", persisted.AccessLevel.ToString());
  }

  [Fact]
  public async Task Handle_WithDuplicateCode_FailsWithoutCreatingSecondRecord()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlite(connection)
        .Options;

    await using var context = new SharedKernelDbContext(options);
    await context.Database.EnsureCreatedAsync();

    var handler = new AddRankCommandHandler(context);

    var first = await handler.Handle(new AddRankCommand("P"), CancellationToken.None);
    var duplicate = await handler.Handle(new AddRankCommand("P"), CancellationToken.None);

    Assert.True(first.IsSuccess);
    Assert.True(duplicate.IsFailure);
    Assert.Equal(ManageRanksErrors.DuplicateCodeName, duplicate.Error.Code);

    var count = await context.Ranks.CountAsync(x => x.Code == "P");
    Assert.Equal(1, count);
  }
}
