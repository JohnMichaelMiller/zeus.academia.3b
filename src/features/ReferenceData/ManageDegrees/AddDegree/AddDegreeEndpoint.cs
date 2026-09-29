using MediatR;
using FluentValidation;
using FluentValidation.Results;
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
      var validationResult = await validator.ValidateAsync(command, ct);
      if (!validationResult.IsValid)
      {
        return Results.ValidationProblem(validationResult.Errors
          .GroupBy(x => x.PropertyName)
          .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToArray()));
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
