using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.Primitives;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

namespace Zeus.Academia.SharedKernel.Domain.Aggregates;

/// <summary>
/// Academic aggregate root. Identified by a 6-character fixed-length
/// employee number (<see cref="EmpNr"/>). Employment status is modeled as
/// tenured XOR contracted — both cannot be set at the same time.
/// <see cref="AccessLevel"/> is derived from <see cref="Rank"/> and never set directly.
/// </summary>
public sealed class Academic : AggregateRoot<string>
{
    /// <summary>Required fixed length of EmpNr.</summary>
    public const int EmpNrLength = 6;

    /// <summary>Maximum length of EmpName.</summary>
    public const int MaxEmpNameLength = 15;

    private readonly List<AcademicQualification> _qualifications = [];

    private Academic(string empNr, string empName, Rank rank) : base(empNr)
    {
        EmpNr = empNr;
        EmpName = empName;
        Rank = rank;
    }

    // EF Core materialization constructor.
    private Academic() : base(string.Empty)
    {
        EmpNr = string.Empty;
        EmpName = string.Empty;
        Rank = Rank.L;
    }

    /// <summary>6-character fixed-length employee number (primary identifier).</summary>
    public string EmpNr { get; private set; }

    /// <summary>Display name, up to 15 characters.</summary>
    public string EmpName { get; private set; }

    /// <summary>Academic rank (P, SL, or L).</summary>
    public Rank Rank { get; private set; }

    /// <summary>Access level derived from <see cref="Rank"/>.</summary>
    public AccessLevel AccessLevel => Rank.ToAccessLevel();

    /// <summary>
    /// Tenure flag. Mutually exclusive with <see cref="ContractEndDate"/>.
    /// <c>null</c> indicates no employment status has been set.
    /// </summary>
    public bool? IsTenured { get; private set; }

    /// <summary>
    /// Contract end date. Mutually exclusive with <see cref="IsTenured"/>.
    /// <c>null</c> indicates no employment status has been set.
    /// </summary>
    public DateOnly? ContractEndDate { get; private set; }

    /// <summary>Extension assigned to this academic, or <c>null</c> if none.</summary>
    public Extension? Extension { get; private set; }

    /// <summary>Academic qualifications (Degree+University pairs) held.</summary>
    public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

    /// <summary>
    /// Creates a new <see cref="Academic"/> with the required identifier, name, and rank.
    /// Employment status is unset at registration time.
    /// </summary>
    public static Academic Register(string empNr, string empName, Rank rank)
    {
        ArgumentNullException.ThrowIfNull(rank);
        ValidateEmpNr(empNr);
        ValidateEmpName(empName);
        return new Academic(empNr, empName, rank);
    }

    /// <summary>Updates the display name.</summary>
    public void Rename(string empName)
    {
        ValidateEmpName(empName);
        EmpName = empName;
    }

    /// <summary>Changes the rank and raises <c>RankChangedEvent</c>.</summary>
    public void ChangeRank(Rank newRank)
    {
        ArgumentNullException.ThrowIfNull(newRank);
        if (Rank == newRank) return;
        var previous = Rank;
        Rank = newRank;
        RaiseDomainEvent(new Events.RankChangedEvent(EmpNr, previous, newRank));
    }

    /// <summary>
    /// Sets the academic as tenured. Clears any contract end date to preserve
    /// the XOR invariant.
    /// </summary>
    public void SetTenured()
    {
        IsTenured = true;
        ContractEndDate = null;
    }

    /// <summary>
    /// Sets the academic as contracted with the given end date. Clears the
    /// tenured flag to preserve the XOR invariant. The end date must be in the future.
    /// </summary>
    public void SetContract(DateOnly contractEndDate, DateOnly today)
    {
        if (contractEndDate <= today)
        {
            throw new BusinessRuleViolationException(
                "Contract end date must be in the future.");
        }
        ContractEndDate = contractEndDate;
        IsTenured = null;
    }

    /// <summary>Clears both tenured flag and contract end date.</summary>
    public void ClearEmploymentStatus()
    {
        IsTenured = null;
        ContractEndDate = null;
    }

    /// <summary>Assigns a provisioned extension to this academic.</summary>
    public void AssignExtension(Extension extension)
    {
        ArgumentNullException.ThrowIfNull(extension);
        Extension = extension;
    }

    /// <summary>Releases the currently assigned extension, if any.</summary>
    public void ReleaseExtension() => Extension = null;

    /// <summary>
    /// Adds an academic qualification (Degree+University pair). Duplicate
    /// (Academic, Degree) pairs are rejected since an academic holds each
    /// degree from at most one university.
    /// </summary>
    public void AddQualification(Degree degree, University university)
    {
        ArgumentNullException.ThrowIfNull(degree);
        ArgumentNullException.ThrowIfNull(university);
        if (_qualifications.Any(q => q.Degree == degree))
        {
            throw new BusinessRuleViolationException(
                $"Academic '{EmpNr}' already holds degree '{degree.Code}'.");
        }
        _qualifications.Add(new AcademicQualification(EmpNr, degree, university));
    }

    /// <summary>Removes a qualification, requiring at least one remains after removal.</summary>
    public void RemoveQualification(Degree degree)
    {
        ArgumentNullException.ThrowIfNull(degree);
        var existing = _qualifications.FirstOrDefault(q => q.Degree == degree)
            ?? throw new BusinessRuleViolationException(
                $"Academic '{EmpNr}' does not hold degree '{degree.Code}'.");
        if (_qualifications.Count == 1)
        {
            throw new BusinessRuleViolationException(
                $"Academic '{EmpNr}' must retain at least one qualification.");
        }
        _qualifications.Remove(existing);
    }

    /// <summary>Raises an <c>AcademicDeregisteredEvent</c> for downstream handlers.</summary>
    public void Deregister() =>
        RaiseDomainEvent(new Events.AcademicDeregisteredEvent(EmpNr));

    private static void ValidateEmpNr(string empNr)
    {
        ArgumentNullException.ThrowIfNull(empNr);
        if (empNr.Length != EmpNrLength)
        {
            throw new ArgumentException(
                $"EmpNr must be exactly {EmpNrLength} characters.",
                nameof(empNr));
        }
    }

    private static void ValidateEmpName(string empName)
    {
        ArgumentNullException.ThrowIfNull(empName);
        var trimmed = empName.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("EmpName must not be empty.", nameof(empName));
        }
        if (empName.Length > MaxEmpNameLength)
        {
            throw new ArgumentException(
                $"EmpName must be at most {MaxEmpNameLength} characters.",
                nameof(empName));
        }
    }
}
