using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.ReferenceData.ManageRanks.ListRanks;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

public static class ManageRanksEndpoints
{
  public static IEndpointRouteBuilder MapManageRanksEndpoints(this IEndpointRouteBuilder app)
  {
    ArgumentNullException.ThrowIfNull(app);

    var group = app.MapGroup("/api/reference-data/ranks");

    group.MapAddRank();
    group.MapListRanks();

    return app;
  }
}
