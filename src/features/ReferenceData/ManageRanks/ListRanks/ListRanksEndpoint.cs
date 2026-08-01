using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public static class ListRanksEndpoint
{
  public static RouteGroupBuilder MapListRanks(this RouteGroupBuilder group)
  {
    group.MapGet("/", async (ISender sender, CancellationToken ct) =>
    {
      var response = await sender.Send(new ListRanksQuery(), ct);
      return Results.Ok(response);
    })
    .WithName("ListRanks")
    .Produces<IReadOnlyList<ListRanksResponse>>(StatusCodes.Status200OK);

    return group;
  }
}
