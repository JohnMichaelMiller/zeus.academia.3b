using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankHandlerTests
{
  [Fact]
  public async Task Handle_WithValidCode_AddsRankAndReturnsMappedAccessLevel()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    await using var context = await CreateContextAsync(connection);
    var handler = new AddRankHandler(context);

    var result = await handler.Handle(new AddRankCommand("SL"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("SL", result.Value.Code);
    Assert.Equal(AccessLevel.NAT, result.Value.AccessLevel);

    var storedCount = await context.Ranks.CountAsync();
    Assert.Equal(1, storedCount);
  }

  [Fact]
  public async Task Handle_WithDuplicateCode_ReturnsDuplicateFailureWithoutCreatingSecondRecord()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    await using var context = await CreateContextAsync(connection);
    context.Ranks.Add(ManagedRank.Create(Rank.P));
    await context.SaveChangesAsync();

    var handler = new AddRankHandler(context);

    var result = await handler.Handle(new AddRankCommand("P"), CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal(ManageRanksErrors.DuplicateCode, result.Error);

    var storedCount = await context.Ranks.CountAsync();
    Assert.Equal(1, storedCount);
  }

  [Fact]
  public async Task Handle_WithInvalidCode_ReturnsInvalidCodeFailure()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    await using var context = await CreateContextAsync(connection);
    var handler = new AddRankHandler(context);

    var result = await handler.Handle(new AddRankCommand("X"), CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal(ManageRanksErrors.InvalidCode, result.Error);

    var storedCount = await context.Ranks.CountAsync();
    Assert.Equal(0, storedCount);
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
