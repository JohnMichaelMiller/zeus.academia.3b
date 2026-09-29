using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.Academics.RegisterAcademic.Persistence;
using Zeus.Academia.Features.Extensions.ProvisionExtension;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.GetDegreeByCode;
using Zeus.Academia.Features.ReferenceData.ManageUniversities.GetUniversityByCode;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public sealed class RegisterAcademicHandler(
  RegisterAcademicDbContext dbContext,
  ISender sender) : IRequestHandler<RegisterAcademicCommand, RegisterAcademicResult>
{
  public async Task<RegisterAcademicResult> Handle(
    RegisterAcademicCommand request,
    CancellationToken cancellationToken)
  {
    if (request.Qualifications is null || request.Qualifications.Count == 0)
    {
      return RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.InvalidAcademicRegistration,
        "qualifications",
        "At least one qualification is required.");
    }

    if (!RankCodeCatalog.TryParseRank(request.RankCode, out var rank))
    {
      return RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.InvalidRankCode,
        "rankCode",
        $"Rank code must be one of {RankCodeCatalog.AllowedValuesMessage}.");
    }

    var (academic, referenceFailure) = await TryCreateAcademicValues(
      request,
      request.Qualifications,
      rank,
      cancellationToken);
    if (referenceFailure is not null)
    {
      return referenceFailure;
    }

    if (academic is null)
    {
      throw new InvalidOperationException("Academic value resolution completed without an academic or a failure.");
    }

    if (await dbContext.Academics.AsNoTracking().AnyAsync(x => x.EmpNr == academic.EmpNr, cancellationToken))
    {
      return RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.AcademicAlreadyExists,
        "empNr",
        "An academic with this employee number already exists.");
    }

    try
    {
      _ = Extension.Create(request.ExtNr);
    }
    catch (ArgumentException)
    {
      return RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.InvalidExtension,
        "extNr",
        "Extension number must be greater than zero.");
    }

    var extension = await dbContext.Extensions
      .SingleOrDefaultAsync(x => x.Number == request.ExtNr, cancellationToken);

    if (extension is null)
    {
      return RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.InvalidExtension,
        "extNr",
        "The extension does not exist.");
    }

    if (!extension.IsAvailable)
    {
      return RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.ExtensionUnavailable,
        "extNr",
        "The extension is already assigned.");
    }

    extension.AssignTo(academic.EmpNr);
    dbContext.Academics.Add(academic);

    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      dbContext.ChangeTracker.Clear();

      if (await dbContext.Academics.AsNoTracking().AnyAsync(x => x.EmpNr == academic.EmpNr, cancellationToken))
      {
        return RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.AcademicAlreadyExists,
          "empNr",
          "An academic with this employee number already exists.");
      }

      var currentExtension = await dbContext.Extensions.AsNoTracking()
        .SingleOrDefaultAsync(x => x.Number == request.ExtNr, cancellationToken);

      if (currentExtension is { IsAvailable: false })
      {
        return RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.ExtensionUnavailable,
          "extNr",
          "The extension is already assigned.");
      }

      throw;
    }

    return RegisterAcademicResult.Success(ToResponse(academic, extension.Number));
  }

  private async Task<(Academic? Academic, RegisterAcademicResult? Failure)> TryCreateAcademicValues(
    RegisterAcademicCommand request,
    IReadOnlyList<RegisterAcademicQualification> inputs,
    Rank rank,
    CancellationToken cancellationToken)
  {
    var qualifications = new List<(Degree degree, University university)>();

    for (var index = 0; index < inputs.Count; index++)
    {
      var qualification = inputs[index];
      if (qualification is null)
      {
        return (null, RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.InvalidAcademicRegistration,
          $"qualifications[{index}]",
          "Qualification is required."));
      }

      Degree degree;
      University university;

      try
      {
        degree = Degree.Create(qualification.DegreeCode ?? string.Empty);
      }
      catch (ArgumentException)
      {
        return (null, RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.InvalidDegree,
          $"qualifications[{index}].degreeCode",
          "Degree code is invalid."));
      }

      var degreeResponse = await sender.Send(new GetDegreeByCodeQuery(degree.Code), cancellationToken);
      if (!degreeResponse.IsFound || degreeResponse.Code is null)
      {
        return (null, RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.InvalidDegree,
          $"qualifications[{index}].degreeCode",
          "Degree code does not exist."));
      }

      try
      {
        university = University.Create(qualification.UniversityCode ?? string.Empty);
      }
      catch (ArgumentException)
      {
        return (null, RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.InvalidUniversity,
          $"qualifications[{index}].universityCode",
          "University code is invalid."));
      }

      var universityResponse = await sender.Send(new GetUniversityByCodeQuery(university.Code), cancellationToken);
      if (!universityResponse.IsFound || universityResponse.Code is null)
      {
        return (null, RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.InvalidUniversity,
          $"qualifications[{index}].universityCode",
          "University code does not exist."));
      }

      if (!universityResponse.IsActive)
      {
        return (null, RegisterAcademicResult.Failure(
          RegisterAcademicErrorCodes.UniversityNotActive,
          $"qualifications[{index}].universityCode",
          "University is inactive."));
      }

      qualifications.Add((Degree.Create(degreeResponse.Code), University.Create(universityResponse.Code)));
    }

    Academic academic;
    try
    {
      academic = Academic.Create(
        request.EmpNr ?? string.Empty,
        request.EmpName ?? string.Empty,
        rank,
        qualifications,
        request.IsTenured,
        request.ContractEndDate);
    }
    catch (ArgumentException exception)
    {
      return (null, RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.InvalidAcademicRegistration,
        "empNr",
        exception.Message));
    }
    catch (BusinessRuleViolationException exception)
    {
      return (null, RegisterAcademicResult.Failure(
        RegisterAcademicErrorCodes.InvalidAcademicRegistration,
        exception.Message.Contains("qualification", StringComparison.OrdinalIgnoreCase)
          ? "qualifications"
          : "empNr",
        exception.Message));
    }

    return (academic, null);
  }

  private static RegisterAcademicResponse ToResponse(Academic academic, int extensionNumber)
  {
    return new RegisterAcademicResponse(
      academic.EmpNr,
      academic.EmpName,
      academic.Rank.ToString(),
      academic.AccessLevel.ToString(),
      academic.IsTenured,
      academic.ContractEndDate,
      Array.AsReadOnly(academic.Qualifications
        .Select(x => new RegisterAcademicQualificationResponse(x.DegreeCode, x.UniversityCode))
        .ToArray()),
      extensionNumber);
  }
}
