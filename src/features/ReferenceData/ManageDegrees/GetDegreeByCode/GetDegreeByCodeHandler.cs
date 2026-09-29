using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;

public sealed class GetDegreeByCodeHandler
  : IRequestHandler<GetDegreeByCodeQuery, GetDegreeByCodeResponse>
{
  private readonly ManageDegreesDbContext _dbContext;

  public GetDegreeByCodeHandler(ManageDegreesDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<GetDegreeByCodeResponse> Handle(
    GetDegreeByCodeQuery request,
    CancellationToken cancellationToken)
  {
    string normalizedCode;

    try
    {
      normalizedCode = Degree.Create(request.Code).Code;
    }
    catch (ArgumentException)
    {
      return new GetDegreeByCodeResponse(false, null);
    }

    var degree = await _dbContext.Degrees
      .AsNoTracking()
      .SingleOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);

    return degree is null
      ? new GetDegreeByCodeResponse(false, null)
      : new GetDegreeByCodeResponse(true, degree.Code);
  }
}
