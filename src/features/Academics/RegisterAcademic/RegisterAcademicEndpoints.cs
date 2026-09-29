using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public static class RegisterAcademicEndpoints
{
  public static IEndpointRouteBuilder MapRegisterAcademicEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/academics").WithTags("Academics");

    group.MapPost("/register", async (
      RegisterAcademicCommand command,
      IValidator<RegisterAcademicCommand> validator,
      ISender sender,
      CancellationToken cancellationToken) =>
    {
      var validationResult = await validator.ValidateAsync(command, cancellationToken);
      if (!validationResult.IsValid)
      {
        return Results.ValidationProblem(ToDictionary(validationResult));
      }

      var result = await sender.Send(command, cancellationToken);
      if (result.IsSuccess)
      {
        return Results.Created($"/api/academics/{result.Value.EmpNr}", result.Value);
      }

      return result.Error.Code switch
      {
        "AcademicAlreadyExists" or "ExtensionUnavailable" => Results.Problem(
          statusCode: StatusCodes.Status409Conflict,
          title: "Conflict",
          detail: result.Error.Message),
        "InvalidRankCode" => ValidationFailure("rankCode", result.Error.Message),
        "InvalidDegree" => ValidationFailure("qualifications.degreeCode", result.Error.Message),
        "InvalidUniversity" or "UniversityNotActive" => ValidationFailure("qualifications.universityCode", result.Error.Message),
        "InvalidExtension" => ValidationFailure("extNr", result.Error.Message),
        "InvalidAcademicRegistration" => ValidationFailure("empNr", result.Error.Message),
        _ => throw new InvalidOperationException($"No HTTP mapping exists for error code '{result.Error.Code}'.")
      };
    })
    .WithName("RegisterAcademic")
    .Produces<RegisterAcademicResponse>(StatusCodes.Status201Created)
    .ProducesValidationProblem()
    .ProducesProblem(StatusCodes.Status409Conflict);

    return app;
  }

  private static IResult ValidationFailure(string field, string message) =>
    Results.ValidationProblem(new Dictionary<string, string[]> { [field] = [message] });

  private static IDictionary<string, string[]> ToDictionary(ValidationResult validationResult) =>
    validationResult.Errors
      .GroupBy(x => x.PropertyName)
      .ToDictionary(x => ToCamelCase(x.Key), x => x.Select(y => y.ErrorMessage).ToArray());

  private static string ToCamelCase(string value) =>
    string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value[1..];
}
