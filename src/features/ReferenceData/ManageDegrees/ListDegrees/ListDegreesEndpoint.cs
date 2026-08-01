using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.ListDegrees;

public static class ListDegreesEndpoint
{
  public static RouteGroupBuilder MapListDegrees(this RouteGroupBuilder group)
  {
    group.MapGet("/", async (ISender sender, CancellationToken ct) =>
    {
      var response = await sender.Send(new ListDegreesQuery(), ct);
      return Results.Ok(response);
    })
    .WithName("ListDegrees")
    .Produces<IReadOnlyList<ListDegreesResponse>>(StatusCodes.Status200OK);

    return group;
  }
}