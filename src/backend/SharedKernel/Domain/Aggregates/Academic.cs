namespace Zeus.Academia.SharedKernel.Domain.Aggregates;

using Zeus.Academia.SharedKernel.Abstractions;
using Zeus.Academia.SharedKernel.Domain.Events;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;
using Zeus.Academia.SharedKernel.Results;

/// <summary>
/// Aggregate root for an Academic employee.
/// <para>
/// Invariants:
/// <list type="bullet">
///   <item><c>EmpNr</c> is exactly 6 characters and is the identifier.</item>
///   <item><c>EmpName</c> is non-empty and ≤ 15 characters.</item>
///   <item><c>AccessLevel</c> is derived from <see cref="Rank"/> and never set directly.</item>
///   <item>Exclusive-or: at most one of <see cref="IsTenured"/> and <see cref="ContractEndDate"/> may be set at any time.</item>
/// </list>
/// </para>
/// </summary>
public sealed class Academic : Entity
{
    public const int EmpNrLength = 6;
    public const int EmpNameMaxLength = 15;

    private Academic(string empNr, string empName, Rank rank)
    {
        EmpNr = empNr;
        EmpName = empName;
        Rank = rank;
    }

    // EF Core materialization.
    private Academic()
    {
        EmpNr = string.Empty;
        EmpName = string.Empty;
        Rank = Rank.L;
    }

    public string EmpNr { get; private set; }

    public string EmpName { get; private set; }

    public Rank Rank { get; private set; }

    /// <summary>Derived access level. Always mirrors <see cref="Rank"/>.</summary>
    public AccessLevel AccessLevel => Rank.AccessLevel;

    public bool? IsTenured { get; private set; }

    public DateOnly? ContractEndDate { get; private set; }

    public Extension? Extension { get; private set; }

    public static Result<Academic> Create(string empNr, string empName, Rank rank)
    {
        var validation = Validate(empNr, empName, rank);
        if (validation.IsFailure)
        {
            return Result<Academic>.Failure(validation.Error);
        }

        var academic = new Academic(empNr, empName.Trim(), rank);
        academic.AddDomainEvent(new AcademicRegisteredEvent(
            academic.EmpNr,
            academic.EmpName,
            academic.Rank,
            DateTimeOffset.UtcNow));

        return Result<Academic>.Success(academic);
    }

    private static Result Validate(string empNr, string empName, Rank rank)
    {
        if (string.IsNullOrWhiteSpace(empNr) || empNr.Length != EmpNrLength)
        {
            return Result.Failure(new Error("Academic.EmpNr.Invalid", $"EmpNr must be exactly {EmpNrLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(empName))
        {
            return Result.Failure(new Error("Academic.EmpName.Empty", "EmpName is required."));
        }

        if (empName.Trim().Length > EmpNameMaxLength)
        {
            return Result.Failure(new Error("Academic.EmpName.TooLong", $"EmpName must be ≤ {EmpNameMaxLength} characters."));
        }

        if (rank is null)
        {
            return Result.Failure(new Error("Academic.Rank.Required", "Rank is required."));
        }

        return Result.Success();
    }

    /// <summary>Grant tenure. Clears any existing contract end date.</summary>
    public void SetTenured()
    {
        IsTenured = true;
        ContractEndDate = null;
    }

    /// <summary>Assign a contract. Clears any existing tenured flag. End date must be in the future.</summary>
    public void SetContract(DateOnly endDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        if (endDate <= today)
        {
            throw new ArgumentOutOfRangeException(nameof(endDate), "Contract end date must be in the future.");
        }

        ContractEndDate = endDate;
        IsTenured = null;
    }

    /// <summary>Clear both tenure and contract, returning the academic to an unclassified employment state.</summary>
    public void RemoveEmploymentStatus()
    {
        IsTenured = null;
        ContractEndDate = null;
    }

    /// <summary>Change the academic rank. AccessLevel is recomputed automatically.</summary>
    public void ChangeRank(Rank newRank)
    {
        ArgumentNullException.ThrowIfNull(newRank);
        if (newRank == Rank)
        {
            return;
        }

        var previous = Rank;
        Rank = newRank;
        AddDomainEvent(new RankChangedEvent(EmpNr, previous, newRank, DateTimeOffset.UtcNow));
    }

    public void UpdateName(string empName)
    {
        if (string.IsNullOrWhiteSpace(empName))
        {
            throw new ArgumentException("EmpName is required.", nameof(empName));
        }

        var trimmed = empName.Trim();
        if (trimmed.Length > EmpNameMaxLength)
        {
            throw new ArgumentException($"EmpName must be ≤ {EmpNameMaxLength} characters.", nameof(empName));
        }

        EmpName = trimmed;
    }

    public void MarkDeregistered()
    {
        AddDomainEvent(new AcademicDeregisteredEvent(EmpNr, DateTimeOffset.UtcNow));
    }
}
