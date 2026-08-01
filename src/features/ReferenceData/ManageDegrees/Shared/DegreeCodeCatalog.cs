using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

public static class DegreeCodeCatalog
{
  public static string NormalizeCode(string? code)
  {
    return Degree.Create(code ?? string.Empty).Code;
  }

  public static bool TryNormalizeCode(string? code, out string normalizedCode)
  {
    try
    {
      normalizedCode = NormalizeCode(code);
      return true;
    }
    catch (ArgumentException)
    {
      normalizedCode = string.Empty;
      return false;
    }
  }
}
