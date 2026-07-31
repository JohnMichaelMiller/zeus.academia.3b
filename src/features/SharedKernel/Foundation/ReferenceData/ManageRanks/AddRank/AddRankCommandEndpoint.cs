using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;

public static class AddRankCommandEndpoint
{
  public static RouteGroupBuilder MapAddRankCommand(this RouteGroupBuilder group)
  {
    ArgumentNullException.ThrowIfNull(group);

    group.MapPost("/", async (AddRankCommand command, ISender sender, CancellationToken cancellationToken) =>
    {
      var result = await sender.Send(command, cancellationToken);

      if (result.IsSuccess)
      {
        return Results.Created($"/api/reference-data/ranks/{result.Value.Code}", result.Value);
      }

      return result.Error.Code == ManageRanksErrors.DuplicateCodeName
          ? Results.Conflict(result.Error)
          : Results.BadRequest(result.Error);
    })
    .WithName("AddRank")
    .Produces<AddRankCommandResponse>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

    return group;
  }
}
