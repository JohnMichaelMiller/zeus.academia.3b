namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed record University
{
  private University(string name)
  {
    Name = name;
  }

  public string Name { get; }

  public static University Create(string name)
  {
    var normalized = Normalize(name);
    if (normalized.Length > SharedKernelFieldLengths.UniversityName)
    {
      throw new ArgumentException($"University name cannot exceed {SharedKernelFieldLengths.UniversityName} characters.", nameof(name));
    }

    return new University(normalized);
  }

  internal static string Normalize(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("University name is required.", nameof(name));
    }

    return name.Trim();
  }
}
