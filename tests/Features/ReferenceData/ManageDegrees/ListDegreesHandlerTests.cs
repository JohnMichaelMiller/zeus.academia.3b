using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.ListDegrees;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class ListDegreesHandlerTests
{
  [Fact]
  public async Task Handle_ReturnsStableSortedCodes()
  {
    await using var dbContext = CreateInMemoryContext();
    dbContext.Degrees.AddRange(
      new DegreeRecord { Code = "MSC" },
      new DegreeRecord { Code = "BSC" },
      new DegreeRecord { Code = "PHD" });
    await dbContext.SaveChangesAsync();

    var handler = new ListDegreesHandler(dbContext);

    var response = await handler.Handle(new ListDegreesQuery(), CancellationToken.None);

    Assert.Equal(3, response.Count);
    Assert.Equal(["BSC", "MSC", "PHD"], response.Select(x => x.Code).ToArray());
  }

  private static ManageDegreesDbContext CreateInMemoryContext()
  {
    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseInMemoryDatabase($"ManageDegreesListTests-{Guid.NewGuid():N}")
      .Options;

    return new ManageDegreesDbContext(options);
  }
}