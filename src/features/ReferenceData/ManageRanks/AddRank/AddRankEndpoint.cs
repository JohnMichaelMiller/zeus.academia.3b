using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public static class AddRankEndpoint
{
  public static RouteGroupBuilder MapAddRankEndpoint(this RouteGroupBuilder group)
  {
    group.MapPost("/", async (AddRankCommand command, ISender sender, CancellationToken cancellationToken) =>
    {
      var result = await sender.Send(command, cancellationToken);

      if (result.IsSuccess)
      {
        return Results.Created($"/api/reference-data/ranks/{result.Value.Code}", result.Value);
      }

      if (result.Error.Code == ManageRanksErrors.DuplicateCode.Code)
      {
        return Results.Conflict(CreateProblem(result.Error, StatusCodes.Status409Conflict));
      }

      return Results.BadRequest(CreateProblem(result.Error, StatusCodes.Status400BadRequest));
    })
    .WithName("AddRank")
    .Produces<AddRankResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict);

    return group;
  }

  private static ProblemDetails CreateProblem(Error error, int statusCode)
  {
    return new ProblemDetails
    {
      Status = statusCode,
      Title = error.Code,
      Detail = error.Description
    };
  }
}
