using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

internal sealed class ManageRanksSqliteTestContext : IAsyncDisposable
{
  private readonly SqliteConnection _connection;

  private ManageRanksSqliteTestContext(SqliteConnection connection)
  {
    _connection = connection;
  }

  public static async Task<ManageRanksSqliteTestContext> CreateAsync()
  {
    var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    var testContext = new ManageRanksSqliteTestContext(connection);

    await using var dbContext = testContext.CreateDbContext();
    await dbContext.Database.EnsureCreatedAsync();

    return testContext;
  }

  public ManageRanksDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
        .UseSqlite(_connection)
        .Options;

    return new ManageRanksDbContext(options);
  }

  public async ValueTask DisposeAsync()
  {
    await _connection.DisposeAsync();
  }
}
