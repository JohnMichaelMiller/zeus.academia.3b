using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.AddDegree;

public static class AddDegreeMappings
{
  public static DegreeRecord ToDegreeRecord(this AddDegreeCommand command)
  {
    var normalizedCode = DegreeCodeCatalog.NormalizeCode(command.Code);

    return new DegreeRecord
    {
      Code = normalizedCode
    };
  }

  public static AddDegreeResponse ToResponse(this DegreeRecord degreeRecord)
  {
    return new AddDegreeResponse(degreeRecord.Code);
  }
}