using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class ManageRanksDbContextModelTests
{
  [Fact]
  public void Ranks_HasPrimaryKeyOnCode_AndNoDuplicateUniqueIndex()
  {
    using var context = CreateContext();
    var entityType = context.Model.FindEntityType(typeof(RankRecord));

    Assert.NotNull(entityType);

    var primaryKey = entityType!.FindPrimaryKey();
    Assert.NotNull(primaryKey);
    Assert.Single(primaryKey!.Properties);
    Assert.Equal("Code", primaryKey.Properties[0].Name);

    var duplicatePkUniqueIndex = entityType.GetIndexes().Any(index =>
        index.IsUnique &&
        index.Properties.Count == primaryKey.Properties.Count &&
        index.Properties.Select(p => p.Name).SequenceEqual(primaryKey.Properties.Select(p => p.Name)));

    Assert.False(duplicatePkUniqueIndex);
  }

  [Fact]
  public void Ranks_HasAllowedCodeCheckConstraint_DerivedFromCanonicalCatalog()
  {
    using var context = CreateContext();

    var createScript = context.Database.GenerateCreateScript();

    Assert.Contains("CK_Ranks_Code_Allowed", createScript, StringComparison.Ordinal);

    foreach (var code in RankCodeCatalog.SupportedCodes)
    {
      Assert.Contains($"'{code}'", createScript, StringComparison.Ordinal);
    }
  }

  [Fact]
  public void SupportedCodes_AreExposedAsImmutableReadOnlyCollection()
  {
    var codes = RankCodeCatalog.SupportedCodes;

    Assert.IsAssignableFrom<IReadOnlyList<string>>(codes);
    Assert.False(codes is string[]);
  }

  private static ManageRanksDbContext CreateContext()
  {
    var connectionString = Environment.GetEnvironmentVariable("ZEUS_SQLSERVER_CONNECTION");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
      if (OperatingSystem.IsWindows())
      {
        connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=ZeusAcademiaManageRanksDesign;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
      }
      else
      {
        throw new InvalidOperationException("ZEUS_SQLSERVER_CONNECTION is required on non-Windows hosts because SQL Server LocalDB is unavailable.");
      }
    }

    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
      .UseSqlServer(connectionString)
      .Options;

    return new ManageRanksDbContext(options);
  }
}
