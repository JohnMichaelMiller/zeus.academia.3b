using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

public static class ListRanksEndpoint
{
  public static RouteGroupBuilder MapListRanksEndpoint(this RouteGroupBuilder group)
  {
    group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
    {
      var result = await sender.Send(new ListRanksQuery(), cancellationToken);

      if (result.IsSuccess)
      {
        return Results.Ok(result.Value);
      }

      return Results.Problem(CreateProblem(result.Error));
    })
    .WithName("ListRanks")
    .Produces<ListRanksResponse>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status500InternalServerError);

    return group;
  }

  private static ProblemDetails CreateProblem(Error error)
  {
    return new ProblemDetails
    {
      Status = StatusCodes.Status500InternalServerError,
      Title = error.Code,
      Detail = error.Description
    };
  }
}
