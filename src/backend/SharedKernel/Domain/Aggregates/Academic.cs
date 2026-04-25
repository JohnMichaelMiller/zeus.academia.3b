namespace Zeus.Academia.SharedKernel.Domain.Aggregates;

using Zeus.Academia.SharedKernel.Domain.Common;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.Exceptions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;

public sealed class Academic : AggregateRoot
{
    private readonly List<AcademicQualification> _qualifications = [];

    // EF Core constructor
    private Academic() { }

    public string   EmpNr    { get; private set; } = default!;
    public string   EmpName  { get; private set; } = default!;
    public string   RankCode { get; private set; } = default!;

    public bool?     IsTenured       { get; private set; }
    public DateOnly? ContractEndDate { get; private set; }

    /// <summary>FK to Extensions table. Null when no extension is assigned.</summary>
    public decimal? ExtensionExtNr { get; private set; }

    public IReadOnlyList<AcademicQualification> Qualifications =>
        _qualifications.AsReadOnly();

    // Derived — never stored
    public Rank        Rank        => Rank.From(RankCode);
    public AccessLevel AccessLevel => Rank.EnsuredAccessLevel;

    public static Academic Create(
        string empNr,
        string empName,
        Rank rank,
        AcademicQualification firstQualification)
    {
        ValidateEmpNr(empNr);
        ValidateEmpName(empName);
        ArgumentNullException.ThrowIfNull(rank);
        ArgumentNullException.ThrowIfNull(firstQualification);

        var academic = new Academic
        {
            EmpNr    = empNr.Trim(),
            EmpName  = empName.Trim(),
            RankCode = rank.Code,
        };
        academic._qualifications.Add(firstQualification);
        return academic;
    }

    public void SetTenured()
    {
        if (ContractEndDate is not null)
            throw new DomainException(
                "Cannot set tenured: a contract end date is already assigned. Remove it first.");
        IsTenured = true;
    }

    public void SetContract(DateOnly endDate)
    {
        if (endDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Contract end date must be in the future.");
        if (IsTenured is true)
            throw new DomainException(
                "Cannot assign a contract: academic is already tenured. Remove tenure first.");
        ContractEndDate = endDate;
    }

    public void RemoveEmploymentStatus()
    {
        IsTenured       = null;
        ContractEndDate = null;
    }

    public void UpdateName(string empName)
    {
        ValidateEmpName(empName);
        EmpName = empName.Trim();
    }

    public void ChangeRank(Rank rank)
    {
        ArgumentNullException.ThrowIfNull(rank);
        RankCode = rank.Code;
    }

    public void AssignExtension(decimal extNr) => ExtensionExtNr = extNr;

    public void ReleaseExtension() => ExtensionExtNr = null;

    public void AddQualification(AcademicQualification qualification)
    {
        ArgumentNullException.ThrowIfNull(qualification);
        if (_qualifications.Any(q =>
                q.DegreeCode     == qualification.DegreeCode &&
                q.UniversityCode == qualification.UniversityCode))
            throw new DomainException(
                $"Qualification {qualification.DegreeCode} from {qualification.UniversityCode} already recorded.");
        _qualifications.Add(qualification);
    }

    public void RemoveQualification(string degreeCode, string universityCode)
    {
        var existing = _qualifications.FirstOrDefault(q =>
            q.DegreeCode     == degreeCode &&
            q.UniversityCode == universityCode)
            ?? throw new DomainException(
                $"Qualification {degreeCode} from {universityCode} not found.");
        _qualifications.Remove(existing);
    }

    private static void ValidateEmpNr(string empNr)
    {
        if (string.IsNullOrWhiteSpace(empNr) || empNr.Trim().Length != 6)
            throw new DomainException("Employee number must be exactly 6 characters.");
    }

    private static void ValidateEmpName(string empName)
    {
        if (string.IsNullOrWhiteSpace(empName) || empName.Trim().Length > 15)
            throw new DomainException("Employee name must be 1–15 characters.");
    }
}
