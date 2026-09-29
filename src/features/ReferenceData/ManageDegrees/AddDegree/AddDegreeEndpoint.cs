using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

public static class AddDegreeEndpoint
{
  public static RouteGroupBuilder MapAddDegree(this RouteGroupBuilder group)
  {
    group.MapPost("/", async (
      AddDegreeCommand command,
      IValidator<AddDegreeCommand> validator,
      ISender sender,
      CancellationToken ct) =>
    {
      var validation = await validator.ValidateAsync(command, ct);
      if (!validation.IsValid)
      {
        var errors = validation.Errors
          .GroupBy(error => error.PropertyName, StringComparer.Ordinal)
          .ToDictionary(
            group => group.Key,
            group => group.Select(error => error.ErrorMessage).Distinct().ToArray(),
            StringComparer.Ordinal);

        return Results.ValidationProblem(errors);
      }

      try
      {
        var response = await sender.Send(command, ct);
        return Results.Created($"/api/reference-data/degrees/{response.Code}", response);
      }
      catch (DegreeConflictException ex)
      {
        return Results.Conflict(new { error = ex.Message });
      }
    })
    .WithName("AddDegree")
    .Produces<AddDegreeResponse>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status409Conflict)
    .ProducesValidationProblem();

    return group;
  }
}
