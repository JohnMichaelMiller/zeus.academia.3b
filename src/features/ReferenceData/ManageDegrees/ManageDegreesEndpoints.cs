using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.ListDegrees;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees;

public static class ManageDegreesEndpoints
{
  public static IEndpointRouteBuilder MapManageDegreesEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/reference-data/degrees");
    group.MapAddDegree();
    group.MapListDegrees();
    return app;
  }
}