using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicHandler
  : IRequestHandler<RegisterAcademicCommand, Result<RegisterAcademicResponse>>
{
  private readonly RegisterAcademicDbContext _dbContext;
  private readonly ISender _sender;

  public RegisterAcademicHandler(RegisterAcademicDbContext dbContext, ISender sender)
  {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<Result<RegisterAcademicResponse>> Handle(
    RegisterAcademicCommand request,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    if (!Academic.TryNormalizeEmpNr(request.EmpNr, out var normalizedEmpNr) || normalizedEmpNr is null)
    {
      return Failure("InvalidAcademicRegistration", "Employee number must be exactly six characters.");
    }

    if (!RankCodeCatalog.TryParseRank(request.RankCode, out var rank))
    {
      return Failure("InvalidRankCode", $"Allowed rank codes: {RankCodeCatalog.AllowedValuesMessage}.");
    }

    if (request.Qualifications is null || request.Qualifications.Count == 0)
    {
      return Failure("InvalidAcademicRegistration", "At least one qualification is required.");
    }

    int extensionNumber;
    try
    {
      extensionNumber = Extension.Create(request.ExtNr).Number;
    }
    catch (ArgumentException exception)
    {
      return Failure("InvalidExtension", exception.Message);
    }

    var qualifications = new List<(Degree degree, University university)>();
    foreach (var qualification in request.Qualifications)
    {
      if (qualification is null || string.IsNullOrWhiteSpace(qualification.DegreeCode))
      {
        return Failure("InvalidDegree", "Degree code is required.");
      }

      var degreeResponse = await _sender.Send(
        new GetDegreeByCodeQuery(qualification.DegreeCode),
        cancellationToken);

      if (!degreeResponse.IsFound || degreeResponse.Code is null)
      {
        return Failure("InvalidDegree", $"Degree '{qualification.DegreeCode.Trim()}' was not found.");
      }

      if (string.IsNullOrWhiteSpace(qualification.UniversityCode))
      {
        return Failure("InvalidUniversity", "University code is required.");
      }

      var universityResponse = await _sender.Send(
        new GetUniversityByCodeQuery(qualification.UniversityCode),
        cancellationToken);

      if (!universityResponse.IsFound || universityResponse.Code is null)
      {
        return Failure("InvalidUniversity", $"University '{qualification.UniversityCode.Trim()}' was not found.");
      }

      if (!universityResponse.IsActive)
      {
        return Failure("UniversityNotActive", $"University '{universityResponse.Code}' is inactive.");
      }

      try
      {
        qualifications.Add((Degree.Create(degreeResponse.Code), University.Create(universityResponse.Code)));
      }
      catch (ArgumentException exception)
      {
        return Failure("InvalidAcademicRegistration", exception.Message);
      }
    }

    Academic academic;
    try
    {
      academic = Academic.Create(
        normalizedEmpNr,
        request.EmpName ?? string.Empty,
        rank,
        qualifications,
        request.IsTenured,
        request.ContractEndDate);
    }
    catch (ArgumentException exception)
    {
      return Failure("InvalidAcademicRegistration", exception.Message);
    }
    catch (BusinessRuleViolationException exception)
    {
      return Failure("InvalidAcademicRegistration", exception.Message);
    }

    if (await _dbContext.Academics.AnyAsync(x => x.EmpNr == academic.EmpNr, cancellationToken))
    {
      return Failure("AcademicAlreadyExists", $"Academic '{academic.EmpNr}' is already registered.");
    }

    await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    var extension = await _dbContext.Extensions
      .SingleOrDefaultAsync(x => x.Number == extensionNumber, cancellationToken);

    if (extension is null)
    {
      return Failure("InvalidExtension", $"Extension '{request.ExtNr}' was not found.");
    }

    if (!extension.IsAvailable)
    {
      return Failure("ExtensionUnavailable", $"Extension '{request.ExtNr}' is already assigned.");
    }

    extension.AssignTo(academic.EmpNr);

    var claimedRows = await _dbContext.Extensions
      .Where(x => x.Number == extensionNumber && x.AssignedEmpNr == null)
      .ExecuteUpdateAsync(
        updates => updates.SetProperty(x => x.AssignedEmpNr, academic.EmpNr),
        cancellationToken);

    if (claimedRows == 0)
    {
      var stillExists = await _dbContext.Extensions
        .AsNoTracking()
        .AnyAsync(x => x.Number == extensionNumber, cancellationToken);

      return stillExists
        ? Failure("ExtensionUnavailable", $"Extension '{extensionNumber}' is already assigned.")
        : Failure("InvalidExtension", $"Extension '{extensionNumber}' was not found.");
    }

    _dbContext.Entry(extension).Property(x => x.AssignedEmpNr).OriginalValue = academic.EmpNr;
    _dbContext.Academics.Add(academic);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
      await transaction.CommitAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      await transaction.RollbackAsync(cancellationToken);
      var duplicateNowExists = await _dbContext.Academics
        .AsNoTracking()
        .AnyAsync(x => x.EmpNr == academic.EmpNr, cancellationToken);

      if (duplicateNowExists)
      {
        return Failure("AcademicAlreadyExists", $"Academic '{academic.EmpNr}' is already registered.");
      }

      throw;
    }

    var responseQualifications = academic.Qualifications
      .Select(x => new RegisterAcademicQualificationResponse(x.DegreeCode, x.UniversityCode))
      .ToList()
      .AsReadOnly();

    return Result<RegisterAcademicResponse>.Success(new RegisterAcademicResponse(
      academic.EmpNr,
      academic.EmpName,
      academic.Rank.ToString(),
      academic.AccessLevel.ToString(),
      academic.IsTenured,
      academic.ContractEndDate,
      responseQualifications,
      extension.Number));
  }

  private static Result<RegisterAcademicResponse> Failure(string code, string message) =>
    Result<RegisterAcademicResponse>.Failure(Error.Create(code, message));
}
