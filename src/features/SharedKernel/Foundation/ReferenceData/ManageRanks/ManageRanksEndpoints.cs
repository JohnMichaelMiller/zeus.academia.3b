using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;
using Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.ListRanks;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks;

public static class ManageRanksEndpoints
{
  public static IEndpointRouteBuilder MapManageRanksEndpoints(this IEndpointRouteBuilder app)
  {
    ArgumentNullException.ThrowIfNull(app);

    var group = app.MapGroup("/api/reference-data/ranks");

    group.MapAddRankCommand();
    group.MapListRanksQuery();

    return app;
  }
}
