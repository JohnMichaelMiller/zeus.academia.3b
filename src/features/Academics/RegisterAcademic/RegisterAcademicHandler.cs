using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicHandler : IRequestHandler<RegisterAcademicCommand, Result<RegisterAcademicResponse>>
{
  private readonly SharedKernelDbContext _dbContext;
  private readonly IMediator _mediator;

  public RegisterAcademicHandler(SharedKernelDbContext dbContext, IMediator mediator)
  {
    _dbContext = dbContext;
    _mediator = mediator;
  }

  public async Task<Result<RegisterAcademicResponse>> Handle(RegisterAcademicCommand request, CancellationToken cancellationToken)
  {
    var normalizedEmpNr = request.EmpNr.Trim();
    var normalizedName = request.EmpName.Trim();

    var universityResponse = await _mediator.Send(new GetUniversityByCodeQuery(request.UniversityCode), cancellationToken);
    if (!universityResponse.IsFound)
    {
      return Result<RegisterAcademicResponse>.Failure(
        Error.Create("InvalidUniversity", $"University '{request.UniversityCode}' was not found."));
    }

    if (!universityResponse.IsActive)
    {
      return Result<RegisterAcademicResponse>.Failure(
        Error.Create("UniversityNotActive", $"University '{universityResponse.Code}' is not active."));
    }

    var rankParseSucceeded = RankCodeCatalog.TryParseRank(request.RankCode, out var rank);
    if (!rankParseSucceeded)
    {
      return Result<RegisterAcademicResponse>.Failure(
        Error.Create("InvalidRankCode", $"Rank code '{request.RankCode}' is not valid."));
    }

    var exists = await _dbContext.Academics
      .AsNoTracking()
      .AnyAsync(x => x.EmpNr == normalizedEmpNr, cancellationToken);

    if (exists)
    {
      return Result<RegisterAcademicResponse>.Failure(
        Error.Create("AcademicAlreadyExists", $"Academic '{normalizedEmpNr}' already exists."));
    }

    try
    {
      var university = University.Create(universityResponse.Code!);
      var degreeCodes = request.DegreeCodes.Select(code => Degree.Create(code)).ToList();
      var academic = Academic.Create(
        normalizedEmpNr,
        normalizedName,
        rank,
        degreeCodes.Select(code => (code, university)).ToList(),
        request.IsTenured,
        request.ContractEndDate);

      _dbContext.Academics.Add(academic);
      await _dbContext.SaveChangesAsync(cancellationToken);

      return Result<RegisterAcademicResponse>.Success(
        new RegisterAcademicResponse(
          academic.EmpNr,
          academic.EmpName,
          academic.Rank.ToString(),
          academic.AccessLevel.ToString(),
          academic.IsTenured,
          academic.Qualifications.Select(x => x.DegreeCode).ToList(),
          university.Code));
    }
    catch (Exception ex) when (ex is ArgumentException or ArgumentOutOfRangeException or BusinessRuleViolationException)
    {
      return Result<RegisterAcademicResponse>.Failure(
        Error.Create("InvalidAcademicRegistration", ex.Message));
    }
  }
}
