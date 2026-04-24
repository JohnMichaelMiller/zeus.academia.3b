using Zeus.Academia.SharedKernel.Domain.Abstractions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;
using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Domain.Aggregates;

/// <summary>
/// Academic aggregate root. Central employment entity of the institution.
/// </summary>
/// <remarks>
/// Invariants enforced here:
/// <list type="bullet">
///   <item><description><see cref="EmpNr"/> is a fixed 6-char identifier, used as the aggregate key.</description></item>
///   <item><description><see cref="EmpName"/> must not exceed 15 characters.</description></item>
///   <item><description><see cref="AccessLevel"/> is derived from <see cref="Rank"/>; never set directly.</description></item>
///   <item><description>Employment state is tenured XOR contracted — never both at once.</description></item>
/// </list>
/// </remarks>
public sealed class Academic : AggregateRoot
{
    public const int MaxEmpNameLength = 15;

    public string EmpNr { get; private set; }

    public string EmpName { get; private set; }

    public Rank Rank { get; private set; }

    /// <summary>Derived from <see cref="Rank"/>. Never settable.</summary>
    public AccessLevel AccessLevel => AccessLevelDerivation.From(Rank);

    public bool? IsTenured { get; private set; }

    public DateOnly? ContractEndDate { get; private set; }

    // EF Core materialization constructor.
    private Academic()
    {
        EmpNr = string.Empty;
        EmpName = string.Empty;
    }

    private Academic(string empNr, string empName, Rank rank)
    {
        EmpNr = empNr;
        EmpName = empName;
        Rank = rank;
    }

    /// <summary>Factory. Creates a new Academic with no employment status set.</summary>
    public static Academic Register(string empNr, string empName, Rank rank)
    {
        var validatedEmpNr = ValueObjects.EmpNr.Create(empNr).Value;
        ValidateEmpName(empName);

        return new Academic(validatedEmpNr, empName, rank);
    }

    /// <summary>Replaces the display name. Enforces the 15-character cap.</summary>
    public void Rename(string newName)
    {
        ValidateEmpName(newName);
        EmpName = newName;
    }

    /// <summary>Changes the rank. <see cref="AccessLevel"/> is recalculated automatically.</summary>
    public void ChangeRank(Rank newRank) => Rank = newRank;

    /// <summary>Marks the Academic as tenured and clears any contract end date.</summary>
    public void SetTenured()
    {
        IsTenured = true;
        ContractEndDate = null;
        AssertEmploymentXor();
    }

    /// <summary>
    /// Assigns a contract end date and clears tenured status.
    /// The date must be in the future relative to <paramref name="today"/>.
    /// </summary>
    public void SetContract(DateOnly contractEndDate, DateOnly today)
    {
        if (contractEndDate <= today)
        {
            throw new BusinessRuleViolationException("Contract end date must be in the future.");
        }

        ContractEndDate = contractEndDate;
        IsTenured = null;
        AssertEmploymentXor();
    }

    /// <summary>Clears both tenured and contracted state.</summary>
    public void RemoveEmploymentStatus()
    {
        IsTenured = null;
        ContractEndDate = null;
    }

    private static void ValidateEmpName(string empName)
    {
        if (string.IsNullOrWhiteSpace(empName))
        {
            throw new BusinessRuleViolationException("EmpName cannot be empty.");
        }

        if (empName.Length > MaxEmpNameLength)
        {
            throw new BusinessRuleViolationException(
                $"EmpName cannot exceed {MaxEmpNameLength} characters.");
        }
    }

    private void AssertEmploymentXor()
    {
        var tenured = IsTenured == true;
        var contracted = ContractEndDate.HasValue;
        if (tenured && contracted)
        {
            throw new BusinessRuleViolationException(
                "Academic cannot be both tenured and contracted at the same time.");
        }
    }
}
