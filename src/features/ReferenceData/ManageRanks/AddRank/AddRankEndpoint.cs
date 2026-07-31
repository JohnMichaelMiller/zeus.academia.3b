using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

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
          : ToProblem(result.Error);
    })
    .WithName("AddRank")
    .Produces<AddRankResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict)
    .ProducesValidationProblem();

    return group;
  }

  private static IResult ToProblem(Error error)
  {
    ArgumentNullException.ThrowIfNull(error);

    if (error == ManageRanksErrors.DuplicateCode)
    {
      return Results.Problem(
          title: "Duplicate rank code",
          detail: error.Description,
          statusCode: StatusCodes.Status409Conflict);
    }

    return Results.Problem(
        title: "Invalid rank request",
        detail: error.Description,
        statusCode: StatusCodes.Status400BadRequest);
  }
}
