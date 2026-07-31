using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

internal static class ManageRanksTestDbContextFactory
{
  public static async Task<ManageRanksDbContext> CreateAsync(SqliteConnection connection)
  {
    ArgumentNullException.ThrowIfNull(connection);

    if (connection.State != System.Data.ConnectionState.Open)
    {
      await connection.OpenAsync();
    }

    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
        .UseSqlite(connection)
        .Options;

    var context = new ManageRanksDbContext(options);
    await context.Database.EnsureCreatedAsync();

    return context;
  }
}
