using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

public static class AddDegreeMappings
{
  public static DegreeRecord ToDegreeRecord(this AddDegreeCommand command)
  {
    if (!DegreeCodeCatalog.TryParseDegree(command.Code, out var degree))
    {
      throw new ArgumentException($"Allowed values: {DegreeCodeCatalog.AllowedValuesMessage}", nameof(command.Code));
    }

    return new DegreeRecord
    {
      Code = degree.Code
    };
  }

  public static AddDegreeResponse ToResponse(this DegreeRecord degreeRecord)
  {
    return new AddDegreeResponse(degreeRecord.Code);
  }
}
