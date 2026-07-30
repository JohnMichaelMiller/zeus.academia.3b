using Zeus.Academia.Features.SharedKernel.Foundation.Exceptions;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public static class SharedKernelNormalization
{
  public static string NormalizeEmpNr(string empNr)
  {
    if (string.IsNullOrWhiteSpace(empNr))
    {
      throw new ArgumentException("Employee number is required.", nameof(empNr));
    }

    var normalized = empNr.Trim().ToUpperInvariant();

    if (normalized.Length != SharedKernelFieldLengths.EmpNr)
    {
      throw new BusinessRuleViolationException($"Employee number must be exactly {SharedKernelFieldLengths.EmpNr} characters.");
    }

    return normalized;
  }

  public static string NormalizeEmpName(string empName)
  {
    if (string.IsNullOrWhiteSpace(empName))
    {
      throw new ArgumentException("Employee name is required.", nameof(empName));
    }

    var normalized = empName.Trim();

    if (normalized.Length > SharedKernelFieldLengths.EmpName)
    {
      throw new BusinessRuleViolationException($"Employee name must be {SharedKernelFieldLengths.EmpName} characters or fewer.");
    }

    return normalized;
  }

  public static string NormalizeCode(string value, string parameterName, string label)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException($"{label} is required.", parameterName);
    }

    var normalized = value.Trim().ToUpperInvariant();

    if (normalized.Length > SharedKernelFieldLengths.Code)
    {
      throw new BusinessRuleViolationException($"{label} must be {SharedKernelFieldLengths.Code} characters or fewer.");
    }

    return normalized;
  }

  public static void EnsureFutureContractDate(DateOnly contractEndDate, DateOnly today)
  {
    if (contractEndDate <= today)
    {
      throw new BusinessRuleViolationException("Contract end date must be in the future.");
    }
  }
}
