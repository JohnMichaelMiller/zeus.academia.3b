using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.ListRanks;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class ManageRanksSqlServerIntegrationTests
{
  [Fact]
  public async Task AddAndListRanks_EnforcesUniqueness_AndReturnsCanonicalRows()
  {
    var connectionString = BuildTestConnectionString();
    var options = new DbContextOptionsBuilder<SharedKernelDbContext>()
        .UseSqlServer(connectionString)
        .Options;

    try
    {
      await using var context = new SharedKernelDbContext(options);
      await context.Database.EnsureDeletedAsync();
      await context.Database.EnsureCreatedAsync();

      var addHandler = new AddRankCommandHandler(context);
      var listHandler = new ListRanksQueryHandler(context);

      var first = await addHandler.Handle(new AddRankCommand("P"), CancellationToken.None);
      var duplicate = await addHandler.Handle(new AddRankCommand("P"), CancellationToken.None);
      var second = await addHandler.Handle(new AddRankCommand("SL"), CancellationToken.None);

      Assert.True(first.IsSuccess);
      Assert.True(second.IsSuccess);
      Assert.True(duplicate.IsFailure);
      Assert.Equal(ManageRanksErrors.DuplicateCodeName, duplicate.Error.Code);

      var listResult = await listHandler.Handle(new ListRanksQuery(), CancellationToken.None);

      Assert.True(listResult.IsSuccess);
      Assert.Collection(
          listResult.Value,
          row =>
          {
            Assert.Equal("P", row.Code);
            Assert.Equal("INT", row.AccessLevel);
          },
          row =>
          {
            Assert.Equal("SL", row.Code);
            Assert.Equal("NAT", row.AccessLevel);
          });
    }
    finally
    {
      try
      {
        await using var cleanupContext = new SharedKernelDbContext(options);
        await cleanupContext.Database.EnsureDeletedAsync();
      }
      catch (Exception cleanupError)
      {
        Console.WriteLine($"Cleanup failed: {cleanupError.Message}");
      }
    }
  }

  private static string BuildTestConnectionString()
  {
    var configuredConnection = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");

    if (string.IsNullOrWhiteSpace(configuredConnection))
    {
      if (!OperatingSystem.IsWindows())
      {
        throw new InvalidOperationException(
            "ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because LocalDB is unavailable.");
      }

      configuredConnection = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    }

    var builder = new SqlConnectionStringBuilder(configuredConnection)
    {
      InitialCatalog = $"ZeusManageRanksTests_{Guid.NewGuid():N}"
    };

    return builder.ConnectionString;
  }
}
