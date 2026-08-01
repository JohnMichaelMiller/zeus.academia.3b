using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

public sealed class AddDegreeHandler : IRequestHandler<AddDegreeCommand, AddDegreeResponse>
{
  private readonly ManageDegreesDbContext _dbContext;

  public AddDegreeHandler(ManageDegreesDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<AddDegreeResponse> Handle(AddDegreeCommand request, CancellationToken cancellationToken)
  {
    var degreeRecord = request.ToDegreeRecord();

    var degreeCodeAlreadyExists = await _dbContext.Degrees
      .AsNoTracking()
      .AnyAsync(x => x.Code == degreeRecord.Code, cancellationToken);

    if (degreeCodeAlreadyExists)
    {
      throw new DegreeConflictException($"Degree code '{degreeRecord.Code}' already exists.");
    }

    _dbContext.Degrees.Add(degreeRecord);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      var degreeCodeNowExists = await _dbContext.Degrees
        .AsNoTracking()
        .AnyAsync(x => x.Code == degreeRecord.Code, cancellationToken);

      if (degreeCodeNowExists)
      {
        throw new DegreeConflictException($"Degree code '{degreeRecord.Code}' already exists.");
      }

      throw;
    }

    return degreeRecord.ToResponse();
  }
}
