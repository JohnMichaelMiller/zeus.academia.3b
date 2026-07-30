using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public static class ListRanksEndpoint
{
  public static RouteGroupBuilder MapListRanks(this RouteGroupBuilder group)
  {
    ArgumentNullException.ThrowIfNull(group);

    group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
    {
      var result = await sender.Send(new ListRanksQuery(), cancellationToken);

      return result.IsSuccess
          ? Results.Ok(result.Value)
          : Results.Problem(title: result.Error.Code, detail: result.Error.Description);
    })
    .WithName("ListRanks")
    .Produces<IReadOnlyList<ListRanksResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status500InternalServerError);

    return group;
  }
}
