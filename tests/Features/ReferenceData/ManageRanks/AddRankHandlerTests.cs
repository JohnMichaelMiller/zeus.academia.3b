using Microsoft.Data.Sqlite;
using Zeus.Academia.Features.ReferenceData.ManageRanks;
using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageRanks;

public sealed class AddRankHandlerTests
{
  [Fact]
  public async Task Handle_WithValidCode_PersistsAndReturnsMappedAccessLevel()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await using var context = await ManageRanksTestDbContextFactory.CreateAsync(connection);
    var sut = new AddRankHandler(context);

    var result = await sut.Handle(new AddRankCommand("sl"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("SL", result.Value.Code);
    Assert.Equal(AccessLevel.NAT, result.Value.AccessLevel);
    Assert.Single(context.Ranks);
  }

  [Fact]
  public async Task Handle_WithDuplicateCode_ReturnsDuplicateFailure()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await using var context = await ManageRanksTestDbContextFactory.CreateAsync(connection);
    var sut = new AddRankHandler(context);

    var first = await sut.Handle(new AddRankCommand("P"), CancellationToken.None);
    var second = await sut.Handle(new AddRankCommand("P"), CancellationToken.None);

    Assert.True(first.IsSuccess);
    Assert.True(second.IsFailure);
    Assert.Equal(ManageRanksErrors.DuplicateCode, second.Error);
    Assert.Single(context.Ranks);
  }

  [Fact]
  public async Task Handle_WithInvalidCode_ReturnsInvalidCodeFailure()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await using var context = await ManageRanksTestDbContextFactory.CreateAsync(connection);
    var sut = new AddRankHandler(context);

    var result = await sut.Handle(new AddRankCommand("X"), CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal(ManageRanksErrors.InvalidCode, result.Error);
    Assert.Empty(context.Ranks);
  }
}
