namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

public sealed class DegreeConflictException : Exception
{
  public DegreeConflictException(string message)
    : base(message)
  {
  }
}