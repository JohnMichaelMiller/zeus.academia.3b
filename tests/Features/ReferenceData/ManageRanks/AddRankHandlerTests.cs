using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class AddRankHandlerTests
{
  [Fact]
  public async Task Handle_WithSupportedCode_PersistsRankAndReturnsAccessLevel()
  {
    await using var testContext = await ManageRanksSqliteTestContext.CreateAsync();
    await using var dbContext = testContext.CreateDbContext();

    var handler = new AddRankHandler(dbContext);

    var result = await handler.Handle(new AddRankCommand("sl"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("SL", result.Value.Code);
    Assert.Equal(AccessLevel.NAT, result.Value.AccessLevel);

    var storedCodes = await dbContext.RankReferences.AsNoTracking().Select(x => x.Code).ToListAsync();

    Assert.Equal(["SL"], storedCodes);
  }

  [Fact]
  public async Task Handle_WithDuplicateCode_ReturnsConflictWithoutCreatingSecondRecord()
  {
    await using var testContext = await ManageRanksSqliteTestContext.CreateAsync();

    await using (var seedContext = testContext.CreateDbContext())
    {
      seedContext.RankReferences.Add(RankReference.Create("P"));
      await seedContext.SaveChangesAsync();
    }

    await using var dbContext = testContext.CreateDbContext();
    var handler = new AddRankHandler(dbContext);

    var result = await handler.Handle(new AddRankCommand("P"), CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal("ManageRanks.DuplicateCode", result.Error.Code);

    var totalRecords = await dbContext.RankReferences.AsNoTracking().CountAsync();
    Assert.Equal(1, totalRecords);
  }
}
