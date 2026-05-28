using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.Primitives;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Domain.Aggregates;

/// <summary>
/// Academic aggregate root.
/// 
/// Invariants enforced in code (and backed by database constraints):
/// 1. EmpNr is exactly 6 characters.
/// 2. EmpName is at most 15 characters.
/// 3. XOR rule: IsTenured and ContractEndDate are mutually exclusive — never both set simultaneously.
/// 4. AccessLevel is derived from Rank only and never set directly.
/// 5. Each Academic uses at most one Extension (unique FK constraint in persistence).
/// </summary>
public sealed class Academic : AggregateRoot
{
    private readonly List<AcademicQualification> _qualifications = [];

    // EF Core parameterless constructor
    private Academic() { }

    // ─── Identity & profile ──────────────────────────────────────────────────

    /// <summary>6-character fixed-length employee number (primary key).</summary>
    public string EmpNr { get; private set; } = default!;

    /// <summary>Employee name (≤ 15 characters).</summary>
    public string EmpName { get; private set; } = default!;

    // ─── Rank & derived access level ─────────────────────────────────────────

    /// <summary>Academic rank. Determines AccessLevel.</summary>
    public Rank Rank { get; private set; }

    /// <summary>
    /// Access level derived exclusively from Rank. Never stored in the database.
    /// P → INT, SL → NAT, L → LOC.
    /// </summary>
    public AccessLevel AccessLevel => Rank switch
    {
        Rank.P  => AccessLevel.INT,
        Rank.SL => AccessLevel.NAT,
        Rank.L  => AccessLevel.LOC,
        _       => throw new InvalidOperationException($"Unknown rank value: {Rank}.")
    };

    // ─── Employment status (XOR) ─────────────────────────────────────────────

    /// <summary>
    /// True when the Academic holds tenure.
    /// Nullable: null means employment status is not set.
    /// XOR with ContractEndDate — both cannot be non-null simultaneously.
    /// </summary>
    public bool? IsTenured { get; private set; }

    /// <summary>
    /// End date of a fixed-term contract.
    /// Nullable: null means no active contract (or academic is tenured instead).
    /// XOR with IsTenured — both cannot be non-null simultaneously.
    /// </summary>
    public DateOnly? ContractEndDate { get; private set; }

    // ─── Relationships ────────────────────────────────────────────────────────

    /// <summary>Telephony extension assigned to this Academic (at most one).</summary>
    public Extension? Extension { get; private set; }

    /// <summary>Degrees obtained by this Academic (at least one required by business rules).</summary>
    public IReadOnlyList<AcademicQualification> Qualifications => _qualifications;

    // ─── Factory ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new Academic, enforcing all structural constraints.
    /// </summary>
    /// <param name="empNr">Exactly 6-character employee number.</param>
    /// <param name="empName">Employee name (≤ 15 characters).</param>
    /// <param name="rank">Initial academic rank.</param>
    /// <returns>A new Academic in employment-status-unset state.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when empNr is not 6 chars or empName exceeds 15 chars.
    /// </exception>
    public static Academic Create(string empNr, string empName, Rank rank)
    {
        ArgumentNullException.ThrowIfNull(empNr);
        ArgumentNullException.ThrowIfNull(empName);

        if (empNr.Length != 6)
            throw new ArgumentException(
                $"EmpNr must be exactly 6 characters (received '{empNr}', length {empNr.Length}).",
                nameof(empNr));

        if (empName.Length > 15)
            throw new ArgumentException(
                $"EmpName must not exceed 15 characters (received length {empName.Length}).",
                nameof(empName));

        return new Academic
        {
            EmpNr   = empNr,
            EmpName = empName,
            Rank    = rank
        };
    }

    // ─── Employment status guards (XOR invariant) ────────────────────────────

    /// <summary>
    /// Grants tenure to this Academic.
    /// Clears ContractEndDate to maintain the XOR invariant.
    /// </summary>
    /// <exception cref="BusinessRuleViolationException">
    /// Thrown when the Academic is already tenured.
    /// </exception>
    public void SetTenured()
    {
        if (IsTenured == true)
            throw new BusinessRuleViolationException(
                $"Academic '{EmpNr}' is already tenured.");

        IsTenured = true;
        ContractEndDate = null;   // XOR: clear the contract side
    }

    /// <summary>
    /// Assigns a fixed-term contract end date.
    /// Clears IsTenured to maintain the XOR invariant.
    /// </summary>
    /// <param name="endDate">A future date on which the contract ends.</param>
    /// <exception cref="BusinessRuleViolationException">
    /// Thrown when the Academic is already tenured, or when endDate is not in the future.
    /// </exception>
    public void SetContract(DateOnly endDate)
    {
        if (IsTenured == true)
            throw new BusinessRuleViolationException(
                $"Academic '{EmpNr}' is tenured — cannot assign a contract end date.");

        if (endDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessRuleViolationException(
                "Contract end date must be a future date.");

        ContractEndDate = endDate;
        IsTenured = null;         // XOR: clear the tenure side
    }

    /// <summary>
    /// Removes all employment-status information (both tenure and contract).
    /// Leaves the Academic in an unclassified employment state.
    /// </summary>
    public void ClearEmploymentStatus()
    {
        IsTenured = null;
        ContractEndDate = null;
    }

    // ─── Rank mutation ────────────────────────────────────────────────────────

    /// <summary>
    /// Updates the Academic's rank. AccessLevel is recalculated automatically.
    /// </summary>
    /// <param name="newRank">The new rank to assign.</param>
    public void ChangeRank(Rank newRank)
    {
        if (Rank == newRank) return;

        Rank previousRank = Rank;
        Rank = newRank;

        RaiseDomainEvent(new Events.RankChangedEvent
        {
            EmpNr        = EmpNr,
            PreviousRank = previousRank,
            NewRank      = newRank
        });
    }

    // ─── Extension assignment ─────────────────────────────────────────────────

    /// <summary>
    /// Assigns a telephony extension to this Academic.
    /// </summary>
    /// <param name="extension">The extension to assign.</param>
    /// <exception cref="ArgumentNullException">Thrown when extension is null.</exception>
    public void AssignExtension(Extension extension)
    {
        ArgumentNullException.ThrowIfNull(extension);
        Extension = extension;
    }

    /// <summary>Removes the currently assigned extension.</summary>
    public void RemoveExtension() => Extension = null;

    // ─── Qualification management ─────────────────────────────────────────────

    /// <summary>
    /// Adds an academic qualification record.
    /// </summary>
    /// <param name="qualification">The qualification to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when qualification is null.</exception>
    public void AddQualification(AcademicQualification qualification)
    {
        ArgumentNullException.ThrowIfNull(qualification);
        _qualifications.Add(qualification);
    }
}
