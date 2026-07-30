using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public static class AddRankEndpoint
{
  public static RouteGroupBuilder MapAddRank(this RouteGroupBuilder group)
  {
    ArgumentNullException.ThrowIfNull(group);

    group.MapPost("/", async (AddRankCommand command, ISender sender, CancellationToken cancellationToken) =>
    {
      var result = await sender.Send(command, cancellationToken);

      return result.IsSuccess
          ? Results.Created($"/api/reference-data/ranks/{result.Value.Code}", result.Value)
          : Results.Problem(
              statusCode: StatusCodes.Status409Conflict,
              title: result.Error.Code,
              detail: result.Error.Description);
    })
    .WithName("AddRank")
    .Produces<AddRankResponse>(StatusCodes.Status201Created)
    .ProducesValidationProblem()
    .ProducesProblem(StatusCodes.Status409Conflict);

    return group;
  }
}
