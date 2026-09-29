using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class GetDegreeByCodeHandlerTests
{
  [Fact]
  public async Task Handle_WithExistingCode_NormalizesAndReturnsDegree()
  {
    await using var dbContext = CreateInMemoryContext();
    dbContext.Degrees.Add(new DegreeRecord { Code = "PHD" });
    await dbContext.SaveChangesAsync();

    var handler = new GetDegreeByCodeHandler(dbContext);

    var response = await handler.Handle(new GetDegreeByCodeQuery(" phd "), CancellationToken.None);

    Assert.True(response.IsFound);
    Assert.Equal("PHD", response.Code);
  }

  [Fact]
  public async Task Handle_WithUnknownCode_ReturnsNotFound()
  {
    await using var dbContext = CreateInMemoryContext();
    var handler = new GetDegreeByCodeHandler(dbContext);

    var response = await handler.Handle(new GetDegreeByCodeQuery("XYZ"), CancellationToken.None);

    Assert.False(response.IsFound);
    Assert.Null(response.Code);
  }

  [Fact]
  public async Task Handle_WithMalformedCode_ReturnsNotFound()
  {
    await using var dbContext = CreateInMemoryContext();
    var handler = new GetDegreeByCodeHandler(dbContext);

    var response = await handler.Handle(new GetDegreeByCodeQuery(" "), CancellationToken.None);

    Assert.False(response.IsFound);
    Assert.Null(response.Code);
  }

  private static ManageDegreesDbContext CreateInMemoryContext()
  {
    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseInMemoryDatabase($"GetDegreeByCodeTests-{Guid.NewGuid():N}")
      .Options;

    return new ManageDegreesDbContext(options);
  }
}
