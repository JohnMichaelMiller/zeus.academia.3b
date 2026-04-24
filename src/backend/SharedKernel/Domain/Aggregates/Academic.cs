using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.Primitives;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Domain.Aggregates;

/// <summary>
/// Academic aggregate root. Identifies an individual academic employee.
///
/// Invariants (all enforced inside this aggregate):
/// <list type="bullet">
///   <item>
///     <description>
///       <c>EmpNr</c> is a fixed 6-character identifier and cannot be changed.
///     </description>
///   </item>
///   <item>
///     <description>
///       <c>EmpName</c> is required and is at most 15 characters.
///     </description>
///   </item>
///   <item>
///     <description>
///       <c>AccessLevel</c> is derived from <see cref="Rank"/> (P→INT, SL→NAT, L→LOC)
///       and is never settable directly.
///     </description>
///   </item>
///   <item>
///     <description>
///       Employment state is mutually exclusive: an academic is either tenured
///       <em>or</em> contracted until a future date, or neither. Both cannot be set
///       at the same time.
///     </description>
///   </item>
///   <item>
///     <description>
///       When contracted, the <c>ContractEndDate</c> must be in the future (UTC today).
///     </description>
///   </item>
/// </list>
/// </summary>
public sealed class Academic : AggregateRoot<string>
{
    /// <summary>Fixed length of the <c>EmpNr</c> identifier.</summary>
    public const int EmpNrLength = 6;

    /// <summary>Maximum length of <c>EmpName</c>.</summary>
    public const int MaxEmpNameLength = 15;

    private Academic(string empNr, string empName, Rank rank) : base(empNr)
    {
        EmpName = empName;
        Rank = rank;
    }

    // EF Core constructor
    private Academic() : base()
    {
        EmpName = null!;
        Rank = null!;
    }

    /// <summary>
    /// Fixed 6-character employee number. Exposed as <see cref="EmpNr"/> and also serves
    /// as the aggregate <see cref="Entity{TId}.Id"/>.
    /// </summary>
    public string EmpNr => Id;

    /// <summary>Employee display name (≤ 15 characters).</summary>
    public string EmpName { get; private set; }

    /// <summary>Academic rank. Drives <see cref="AccessLevel"/>.</summary>
    public Rank Rank { get; private set; }

    /// <summary>Derived access level; computed from <see cref="Rank"/> only.</summary>
    public AccessLevel AccessLevel => Rank.ToAccessLevel();

    /// <summary>
    /// Whether this academic is tenured. Mutually exclusive with
    /// <see cref="ContractEndDate"/>; <c>null</c> if neither applies.
    /// </summary>
    public bool? IsTenured { get; private set; }

    /// <summary>
    /// Date until which this academic is contracted. Mutually exclusive with
    /// <see cref="IsTenured"/>; <c>null</c> if not under contract.
    /// </summary>
    public DateOnly? ContractEndDate { get; private set; }

    /// <summary>Currently-assigned telephone extension, if any.</summary>
    public Extension? Extension { get; private set; }

    /// <summary>
    /// Registers a new academic. Validates <paramref name="empNr"/> and
    /// <paramref name="empName"/> length.
    /// </summary>
    public static Academic Register(string empNr, string empName, Rank rank)
    {
        ArgumentNullException.ThrowIfNull(rank);

        if (string.IsNullOrWhiteSpace(empNr) || empNr.Length != EmpNrLength)
        {
            throw new BusinessRuleViolationException(
                "Academic.EmpNrInvalid",
                $"EmpNr must be exactly {EmpNrLength} characters.");
        }

        if (string.IsNullOrWhiteSpace(empName))
        {
            throw new BusinessRuleViolationException(
                "Academic.EmpNameRequired", "EmpName is required.");
        }

        if (empName.Length > MaxEmpNameLength)
        {
            throw new BusinessRuleViolationException(
                "Academic.EmpNameTooLong",
                $"EmpName cannot exceed {MaxEmpNameLength} characters.");
        }

        return new Academic(empNr, empName, rank);
    }

    /// <summary>Updates the employee name. Enforces the 15-character limit.</summary>
    public void Rename(string empName)
    {
        if (string.IsNullOrWhiteSpace(empName))
        {
            throw new BusinessRuleViolationException(
                "Academic.EmpNameRequired", "EmpName is required.");
        }

        if (empName.Length > MaxEmpNameLength)
        {
            throw new BusinessRuleViolationException(
                "Academic.EmpNameTooLong",
                $"EmpName cannot exceed {MaxEmpNameLength} characters.");
        }

        EmpName = empName;
    }

    /// <summary>
    /// Assigns a new rank. <see cref="AccessLevel"/> is recomputed automatically.
    /// </summary>
    public void ChangeRank(Rank newRank)
    {
        ArgumentNullException.ThrowIfNull(newRank);
        Rank = newRank;
    }

    /// <summary>
    /// Marks the academic as tenured. Clears any existing contract end date to
    /// preserve the XOR invariant.
    /// </summary>
    public void SetTenured()
    {
        IsTenured = true;
        ContractEndDate = null;
    }

    /// <summary>
    /// Puts the academic on a contract until <paramref name="contractEndDate"/>
    /// (must be in the future). Clears tenure to preserve the XOR invariant.
    /// </summary>
    public void SetContract(DateOnly contractEndDate)
    {
        SetContract(contractEndDate, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    /// <summary>
    /// Test-friendly overload allowing the caller to supply the reference date used
    /// for the future-date check.
    /// </summary>
    public void SetContract(DateOnly contractEndDate, DateOnly today)
    {
        if (contractEndDate <= today)
        {
            throw new BusinessRuleViolationException(
                "Academic.ContractEndDateNotFuture",
                "ContractEndDate must be in the future.");
        }

        ContractEndDate = contractEndDate;
        IsTenured = null;
    }

    /// <summary>
    /// Clears both tenure and contract state, leaving the academic with no
    /// employment-status flag set. Used by <c>RemoveEmploymentStatus</c>.
    /// </summary>
    public void ClearEmploymentStatus()
    {
        IsTenured = null;
        ContractEndDate = null;
    }

    /// <summary>Assigns a telephone extension to the academic.</summary>
    public void AssignExtension(Extension extension)
    {
        ArgumentNullException.ThrowIfNull(extension);
        Extension = extension;
    }

    /// <summary>Releases the currently-assigned telephone extension (if any).</summary>
    public void ReleaseExtension()
    {
        Extension = null;
    }
}
