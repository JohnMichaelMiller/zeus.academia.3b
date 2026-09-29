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
      var validationResult = await validator.ValidateAsync(command, ct);
      if (!validationResult.IsValid)
      {
        return Results.ValidationProblem(validationResult.Errors
          .GroupBy(error => error.PropertyName)
          .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(error => error.ErrorMessage).ToArray()));
      }

      try
      {
        var response = await sender.Send(command, ct);
        return Results.Created($"/api/reference-data/degrees/{response.Code}", response);
      }
      catch (ArgumentException ex) when (ex.ParamName == nameof(AddDegreeCommand.Code))
      {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
          [nameof(AddDegreeCommand.Code)] = [ex.Message]
        });
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
