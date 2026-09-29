using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public static class RegisterAcademicEndpoints
{
  public static IEndpointRouteBuilder MapRegisterAcademicEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/academics/register", async (
      RegisterAcademicCommand command,
      IValidator<RegisterAcademicCommand> validator,
      ISender sender,
      CancellationToken cancellationToken) =>
    {
      var validation = await validator.ValidateAsync(command, cancellationToken);
      if (!validation.IsValid)
      {
        var errors = validation.Errors
          .GroupBy(error => ToRequestField(error.PropertyName), StringComparer.Ordinal)
          .ToDictionary(
            group => group.Key,
            group => group.Select(error => error.ErrorMessage).Distinct().ToArray(),
            StringComparer.Ordinal);

        return Results.ValidationProblem(errors);
      }

      var result = await sender.Send(command, cancellationToken);
      if (result.IsSuccess && result.Response is not null)
      {
        return Results.Created($"/api/academics/{result.Response.EmpNr}", result.Response);
      }

      return result.ErrorCode switch
      {
        RegisterAcademicErrorCodes.AcademicAlreadyExists or RegisterAcademicErrorCodes.ExtensionUnavailable =>
          Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: result.ErrorCode,
            detail: result.Message),
        RegisterAcademicErrorCodes.InvalidRankCode
          or RegisterAcademicErrorCodes.InvalidDegree
          or RegisterAcademicErrorCodes.InvalidUniversity
          or RegisterAcademicErrorCodes.UniversityNotActive
          or RegisterAcademicErrorCodes.InvalidExtension
          or RegisterAcademicErrorCodes.InvalidAcademicRegistration =>
          Results.ValidationProblem(new Dictionary<string, string[]>
          {
            [result.Field ?? "request"] = [result.Message ?? "The request is invalid."]
          }),
        _ => throw new InvalidOperationException(
          $"RegisterAcademic returned an unmapped error code '{result.ErrorCode ?? "<null>"}'.")
      };
    })
    .WithName("RegisterAcademic")
    .Produces<RegisterAcademicResponse>(StatusCodes.Status201Created)
    .ProducesValidationProblem()
    .ProducesProblem(StatusCodes.Status409Conflict);

    return app;
  }

  private static string ToRequestField(string propertyName)
  {
    if (propertyName.StartsWith("Qualifications[", StringComparison.Ordinal))
    {
      var nestedField = propertyName["Qualifications".Length..]
        .Replace(".DegreeCode", ".degreeCode", StringComparison.Ordinal)
        .Replace(".UniversityCode", ".universityCode", StringComparison.Ordinal);
      return string.Concat("qualifications", nestedField);
    }

    return propertyName switch
    {
      "EmpNr" => "empNr",
      "EmpName" => "empName",
      "RankCode" => "rankCode",
      "Qualifications" => "qualifications",
      "ExtNr" => "extNr",
      "ContractEndDate" => "contractEndDate",
      _ => propertyName
    };
  }
}
