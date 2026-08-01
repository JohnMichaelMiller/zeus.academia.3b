using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public static class AddRankEndpoint
{
  public static RouteGroupBuilder MapAddRank(this RouteGroupBuilder group)
  {
    group.MapPost("/", async (AddRankCommand command, ISender sender, CancellationToken ct) =>
    {
      try
      {
        var response = await sender.Send(command, ct);
        return Results.Created($"/api/reference-data/ranks/{response.Code}", response);
      }
      catch (RankConflictException ex)
      {
        return Results.Conflict(new { error = ex.Message });
      }
    })
    .WithName("AddRank")
    .Produces<AddRankResponse>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status409Conflict)
    .ProducesValidationProblem();

    return group;
  }
}
