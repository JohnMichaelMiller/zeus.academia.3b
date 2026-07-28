using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Domain.Entities;

/// <summary>
/// Canonical, persistence-backed degree reference-data entry.
/// </summary>
public sealed class DegreeCatalogEntry
{
    // EF Core parameterless constructor
    private DegreeCatalogEntry() { }

    /// <summary>Unique degree code.</summary>
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Creates a normalized catalog entry from an input code.
    /// </summary>
    /// <param name="code">Raw degree code input.</param>
    /// <exception cref="ArgumentException">Thrown when code is empty or exceeds max length.</exception>
    public static DegreeCatalogEntry Create(string code)
    {
        string normalizedCode = Normalize(code);
        return new DegreeCatalogEntry { Code = normalizedCode };
    }

    /// <summary>
    /// Normalizes degree code for canonical storage and comparison.
    /// </summary>
    /// <param name="code">Raw degree code input.</param>
    /// <returns>Trimmed, uppercase degree code.</returns>
    /// <exception cref="ArgumentException">Thrown when code is empty or exceeds max length.</exception>
    public static string Normalize(string code)
    {
        ArgumentNullException.ThrowIfNull(code);

        string normalizedCode = code.Trim().ToUpperInvariant();
        if (normalizedCode.Length == 0)
            throw new ArgumentException("Degree code must not be empty.", nameof(code));

        if (normalizedCode.Length > Degree.MaxCodeLength)
            throw new ArgumentException(
                $"Degree code must not exceed {Degree.MaxCodeLength} characters.", nameof(code));

        return normalizedCode;
    }
}
