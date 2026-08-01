using System.Collections.ObjectModel;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;

public static class DegreeCodeCatalog
{
  private static readonly ReadOnlyCollection<string> SupportedCodesCollection =
    Array.AsReadOnly(["BSC", "MCS", "PHD"]);

  public static IReadOnlyList<string> SupportedCodes => SupportedCodesCollection;

  public static bool IsAllowed(string? code, out string normalizedCode)
  {
    normalizedCode = NormalizeCode(code);
    return SupportedCodesCollection.Contains(normalizedCode, StringComparer.Ordinal);
  }

  public static bool TryParseDegree(string? code, out Degree degree)
  {
    degree = null!;

    if (!IsAllowed(code, out var normalizedCode))
    {
      return false;
    }

    degree = Degree.Create(normalizedCode);
    return true;
  }

  public static string AllowedValuesMessage => string.Join(", ", SupportedCodesCollection);

  public static string NormalizeCode(string? code)
  {
    return string.IsNullOrWhiteSpace(code) ? string.Empty : code.Trim().ToUpperInvariant();
  }
}
