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
    var group = app.MapGroup("/api/academics");
    group.MapPost("/register", async (RegisterAcademicCommand command, IValidator<RegisterAcademicCommand> validator, ISender sender, CancellationToken ct) =>
    {
      var validationResult = await validator.ValidateAsync(command, ct);
      if (!validationResult.IsValid)
      {
        return Results.ValidationProblem(validationResult.Errors
          .GroupBy(error => error.PropertyName)
          .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(error => error.ErrorMessage).ToArray()));
      }

      var result = await sender.Send(command, ct);
      return result.IsSuccess
        ? Results.Created($"/api/academics/{result.Value.EmpNr}", result.Value)
        : Results.Problem(result.Error.Message, statusCode: StatusCodes.Status400BadRequest);
    })
    .WithName("RegisterAcademic")
    .Produces<RegisterAcademicResponse>(StatusCodes.Status201Created)
    .ProducesValidationProblem();

    return app;
  }
}
