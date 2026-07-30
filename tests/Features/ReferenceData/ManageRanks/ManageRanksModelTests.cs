using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class ManageRanksModelTests
{
  [Fact]
  public async Task Database_RejectsCodeOutsideRankCatalog()
  {
    await using var testContext = await ManageRanksSqliteTestContext.CreateAsync();
    await using var dbContext = testContext.CreateDbContext();

    var exception = await Assert.ThrowsAsync<SqliteException>(() =>
        dbContext.Database.ExecuteSqlRawAsync("INSERT INTO RankReferences (Code) VALUES ('X')"));

    Assert.Contains("constraint", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public async Task GenerateCreateScript_IncludesCanonicalRankCodesInCheckConstraint()
  {
    await using var testContext = await ManageRanksSqliteTestContext.CreateAsync();
    await using var dbContext = testContext.CreateDbContext();

    var createScript = dbContext.Database.GenerateCreateScript();

    foreach (var code in RankCatalog.SupportedCodes)
    {
      Assert.Contains($"'{code}'", createScript);
    }
  }
}
