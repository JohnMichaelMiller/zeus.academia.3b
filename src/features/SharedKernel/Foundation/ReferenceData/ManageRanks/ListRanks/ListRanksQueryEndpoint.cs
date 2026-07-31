using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.ListRanks;

public static class ListRanksQueryEndpoint
{
  public static RouteGroupBuilder MapListRanksQuery(this RouteGroupBuilder group)
  {
    ArgumentNullException.ThrowIfNull(group);

    group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
    {
      var result = await sender.Send(new ListRanksQuery(), cancellationToken);

      return result.IsSuccess
          ? Results.Ok(result.Value)
          : Results.Problem(result.Error.Description, title: result.Error.Code);
    })
    .WithName("ListRanks")
    .Produces<IReadOnlyList<ListRanksQueryResponse>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);

    return group;
  }
}
