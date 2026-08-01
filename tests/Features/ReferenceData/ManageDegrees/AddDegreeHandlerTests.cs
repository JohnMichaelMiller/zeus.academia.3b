using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Tests.Features.ReferenceData.ManageDegrees;

public sealed class AddDegreeHandlerTests
{
  [Fact]
  public async Task Handle_WhenCodeAllowed_PersistsDegreeWithNormalizedCode()
  {
    await using var dbContext = CreateInMemoryContext();
    var handler = new AddDegreeHandler(dbContext);

    var response = await handler.Handle(new AddDegreeCommand(" phd "), CancellationToken.None);

    Assert.Equal("PHD", response.Code);

    var persisted = await dbContext.Degrees.SingleAsync(x => x.Code == "PHD");
    Assert.Equal("PHD", persisted.Code);
  }

  [Fact]
  public async Task Handle_WhenDuplicateCodeExists_ThrowsDegreeConflictException()
  {
    await using var dbContext = CreateInMemoryContext();
    dbContext.Degrees.Add(new DegreeRecord { Code = "PHD" });
    await dbContext.SaveChangesAsync();

    var handler = new AddDegreeHandler(dbContext);

    var exception = await Assert.ThrowsAsync<DegreeConflictException>(async () =>
      await handler.Handle(new AddDegreeCommand(" phd "), CancellationToken.None));

    Assert.Contains("already exists", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  private static ManageDegreesDbContext CreateInMemoryContext()
  {
    var options = new DbContextOptionsBuilder<ManageDegreesDbContext>()
      .UseInMemoryDatabase($"ManageDegreesTests-{Guid.NewGuid():N}")
      .Options;

    return new ManageDegreesDbContext(options);
  }
}