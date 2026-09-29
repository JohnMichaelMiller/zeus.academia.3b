using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.Academics.RegisterAcademic.Register;

public sealed class RegisterAcademicHandler : IRequestHandler<RegisterAcademicCommand, Result<RegisterAcademicResponse>>
{
  private readonly RegisterAcademicDbContext _dbContext;
  private readonly IMediator _mediator;

  public RegisterAcademicHandler(RegisterAcademicDbContext dbContext, IMediator mediator)
  {
    _dbContext = dbContext;
    _mediator = mediator;
  }

  public async Task<Result<RegisterAcademicResponse>> Handle(
    RegisterAcademicCommand request,
    CancellationToken cancellationToken)
  {
    try
    {
      if (!RankCodeCatalog.TryParseRank(request.RankCode, out var rank))
      {
        return Result<RegisterAcademicResponse>.Failure(
          Error.Create("InvalidRankCode", $"Allowed values: {RankCodeCatalog.AllowedValuesMessage}"));
      }

      if (request.Qualifications is null || request.Qualifications.Count == 0)
      {
        return Result<RegisterAcademicResponse>.Failure(
          Error.Create("InvalidAcademicRegistration", "At least one qualification is required."));
      }

      var normalizedEmpNr = Academic.NormalizeEmpNr(request.EmpNr);
      var normalizedName = Academic.NormalizeEmpName(request.EmpName);
      var seenPairs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var qualificationItems = new List<(Degree degree, University university)>();

      foreach (var qualification in request.Qualifications)
      {
        var degreeResponse = await _mediator.Send(new GetDegreeByCodeQuery(qualification.DegreeCode), cancellationToken);
        if (!degreeResponse.IsFound || string.IsNullOrWhiteSpace(degreeResponse.Code))
        {
          return Result<RegisterAcademicResponse>.Failure(
            Error.Create("InvalidDegree", $"Degree code '{qualification.DegreeCode}' was not found."));
        }

        var universityResponse = await _mediator.Send(new GetUniversityByCodeQuery(qualification.UniversityCode), cancellationToken);
        if (!universityResponse.IsFound || string.IsNullOrWhiteSpace(universityResponse.Code))
        {
          return Result<RegisterAcademicResponse>.Failure(
            Error.Create("InvalidUniversity", $"University code '{qualification.UniversityCode}' was not found."));
        }

        if (!universityResponse.IsActive)
        {
          return Result<RegisterAcademicResponse>.Failure(
            Error.Create("UniversityNotActive", $"University code '{qualification.UniversityCode}' is not active."));
        }

        var degree = Degree.Create(degreeResponse.Code);
        var university = University.Create(universityResponse.Code);
        var pairKey = $"{degree.Code}|{university.Code}";
        if (!seenPairs.Add(pairKey))
        {
          return Result<RegisterAcademicResponse>.Failure(
            Error.Create("InvalidAcademicRegistration", "Qualification pairs must be unique."));
        }

        qualificationItems.Add((degree, university));
      }

      var extension = await _dbContext.Extensions
        .SingleOrDefaultAsync(x => x.Number == request.ExtNr, cancellationToken);

      if (extension is null)
      {
        return Result<RegisterAcademicResponse>.Failure(
          Error.Create("InvalidExtension", $"Extension number '{request.ExtNr}' was not found."));
      }

      if (!extension.IsAvailable)
      {
        return Result<RegisterAcademicResponse>.Failure(
          Error.Create("ExtensionUnavailable", $"Extension number '{request.ExtNr}' is already assigned."));
      }

      var academicExists = await _dbContext.Academics
        .AsNoTracking()
        .AnyAsync(x => x.EmpNr == normalizedEmpNr, cancellationToken);

      if (academicExists)
      {
        return Result<RegisterAcademicResponse>.Failure(
          Error.Create("AcademicAlreadyExists", $"Academic '{normalizedEmpNr}' is already registered."));
      }

      var academic = Academic.Create(
        normalizedEmpNr,
        normalizedName,
        rank,
        qualificationItems,
        request.IsTenured,
        request.ContractEndDate);

      extension.AssignTo(normalizedEmpNr);
      _dbContext.Academics.Add(academic);
      await _dbContext.SaveChangesAsync(cancellationToken);

      var response = new RegisterAcademicResponse(
        academic.EmpNr,
        academic.EmpName,
        academic.Rank.ToString(),
        academic.AccessLevel.ToString(),
        academic.IsTenured,
        academic.ContractEndDate,
        academic.Qualifications
          .Select(x => new RegisterAcademicQualificationResponse(x.DegreeCode, x.UniversityCode))
          .ToArray(),
        extension.Number);

      return Result<RegisterAcademicResponse>.Success(response);
    }
    catch (BusinessRuleViolationException ex)
    {
      return Result<RegisterAcademicResponse>.Failure(
        Error.Create("InvalidAcademicRegistration", ex.Message));
    }
    catch (ArgumentException ex)
    {
      return Result<RegisterAcademicResponse>.Failure(
        Error.Create("InvalidAcademicRegistration", ex.Message));
    }
  }
}
