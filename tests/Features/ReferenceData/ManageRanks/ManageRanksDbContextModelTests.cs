using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class ManageRanksDbContextModelTests
{
  [Fact]
  public void RankReference_HasPrimaryKeyOnCode()
  {
    using var context = CreateContext();
    var entityType = context.Model.FindEntityType("Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence.RankReference");

    Assert.NotNull(entityType);

    var primaryKey = entityType!.FindPrimaryKey();

    Assert.NotNull(primaryKey);
    Assert.Single(primaryKey!.Properties);
    Assert.Equal("Code", primaryKey.Properties[0].Name);
  }

  [Fact]
  public void RankReference_HasAllowedCodesCheckConstraint_DerivedFromCanonicalCatalog()
  {
    using var context = CreateContext();

    var createScript = context.Database.GenerateCreateScript();

    Assert.Contains("CK_Ranks_Code_Allowed", createScript, StringComparison.Ordinal);

    foreach (var code in RankCatalog.AllowedCodes)
    {
      Assert.Contains($"'{code}'", createScript, StringComparison.Ordinal);
    }
  }

  private static ManageRanksDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<ManageRanksDbContext>()
        .UseSqlite("Data Source=:memory:")
        .Options;

    return new ManageRanksDbContext(options);
  }
}
