using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MediatR;
using Zeus.Academia.Features.Academics.RegisterAcademic.Register;

namespace Zeus.Academia.Features.Academics.RegisterAcademic;

public static class RegisterAcademicEndpoints
{
  public static IEndpointRouteBuilder MapRegisterAcademicEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/academics");
    group.MapPost("/register", async (
      RegisterAcademicCommand request,
      IMediator mediator,
      CancellationToken cancellationToken) =>
    {
      var result = await mediator.Send(request, cancellationToken);

      if (result.IsFailure)
      {
        return result.Error.Code switch
        {
          "InvalidAcademicRegistration" => Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
              [nameof(RegisterAcademicCommand.EmpNr)] = [result.Error.Message],
            }),
          "InvalidRankCode" => Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
              [nameof(RegisterAcademicCommand.RankCode)] = [result.Error.Message],
            }),
          "InvalidDegree" => Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
              ["degreeCode"] = [result.Error.Message],
            }),
          "InvalidUniversity" or "UniversityNotActive" => Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
              ["universityCode"] = [result.Error.Message],
            }),
          "InvalidExtension" => Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
              [nameof(RegisterAcademicCommand.ExtNr)] = [result.Error.Message],
            }),
          "AcademicAlreadyExists" or "ExtensionUnavailable" => Results.Problem(
            detail: result.Error.Message,
            statusCode: StatusCodes.Status409Conflict),
          _ => Results.BadRequest(result.Error.Message)
        };
      }

      return Results.Created($"/api/academics/{result.Value.EmpNr}", result.Value);
    })
      .WithName("RegisterAcademic")
      .Accepts<RegisterAcademicCommand>("application/json")
      .Produces<RegisterAcademicResponse>(StatusCodes.Status201Created)
      .ProducesValidationProblem()
      .ProducesProblem(StatusCodes.Status409Conflict)
      .Produces(StatusCodes.Status400BadRequest);

    return app;
  }
}
