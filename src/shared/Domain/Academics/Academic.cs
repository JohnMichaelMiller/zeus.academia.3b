using Zeus.Academia.Shared.Abstractions;
using Zeus.Academia.Shared.Domain.Academics.Events;
using Zeus.Academia.Shared.Domain.Exceptions;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Shared.Domain.Academics;

public sealed class Academic : Entity
{
    private readonly List<AcademicQualification> _qualifications = new();

    private Academic()
        : base()
    {
        EmpNr = null!;
        EmpName = null!;
        Rank = null!;
    }

    private Academic(
        Guid id,
        EmpNr empNr,
        EmpName empName,
        Rank rank,
        Extension? extension)
        : base(id)
    {
        EmpNr = empNr;
        EmpName = empName;
        Rank = rank;
        IsTenured = false;
        ContractEndDate = null;
        Extension = extension;
    }

    public EmpNr EmpNr { get; private set; }

    public EmpName EmpName { get; private set; }

    public Rank Rank { get; private set; }

    // AccessLevel is never persisted or set directly; it is derived from Rank.
    public AccessLevel AccessLevel => AccessLevel.FromRank(Rank);

    public bool IsTenured { get; private set; }

    public DateOnly? ContractEndDate { get; private set; }

    public Extension? Extension { get; private set; }

    public IReadOnlyCollection<AcademicQualification> Qualifications => _qualifications.AsReadOnly();

    public static Result<Academic> Register(
        EmpNr empNr,
        EmpName empName,
        Rank rank,
        IEnumerable<(Degree degree, University university)> qualifications,
        Extension? extension = null)
    {
        if (qualifications is null)
        {
            return Result<Academic>.Failure(
                Error.Validation("At least one qualification is required to register an academic."));
        }

        var items = qualifications.ToList();
        if (items.Count == 0)
        {
            return Result<Academic>.Failure(
                Error.Validation("At least one qualification is required to register an academic."));
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (degree, _) in items)
        {
            if (!seen.Add(degree.Code))
            {
                return Result<Academic>.Failure(
                    Error.Validation($"Duplicate degree code '{degree.Code}' is not permitted."));
            }
        }

        var academic = new Academic(Guid.NewGuid(), empNr, empName, rank, extension);
        foreach (var (degree, university) in items)
        {
            academic._qualifications.Add(AcademicQualification.Create(academic.Id, degree, university));
        }

        academic.Raise(new AcademicRegistered(academic.Id, empNr.Value, rank.Code));
        return academic;
    }

    public Result UpdateName(EmpName newName)
    {
        EmpName = newName;
        Raise(new AcademicNameUpdated(Id, newName.Value));
        return Result.Success();
    }

    public Result ChangeRank(Rank newRank)
    {
        var oldRank = Rank;
        var oldAccess = AccessLevel;
        Rank = newRank;
        Raise(new RankChanged(Id, oldRank.Code, newRank.Code, oldAccess.Code, AccessLevel.Code));
        return Result.Success();
    }

    public Result GrantTenure()
    {
        if (IsTenured)
        {
            return Result.Failure(Error.Conflict("Academic is already tenured."));
        }

        IsTenured = true;
        ContractEndDate = null;
        EnsureEmploymentXor();
        Raise(new TenureGranted(Id));
        return Result.Success();
    }

    public Result AssignContract(DateOnly endDate, DateOnly today)
    {
        if (endDate <= today)
        {
            return Result.Failure(Error.Validation("Contract end date must be strictly in the future."));
        }

        ContractEndDate = endDate;
        IsTenured = false;
        EnsureEmploymentXor();
        Raise(new ContractAssigned(Id, endDate));
        return Result.Success();
    }

    public Result RenewContract(DateOnly newEndDate, DateOnly today)
    {
        if (ContractEndDate is null)
        {
            return Result.Failure(Error.Conflict("Academic is not currently under a contract."));
        }

        if (newEndDate <= today)
        {
            return Result.Failure(Error.Validation("Contract end date must be strictly in the future."));
        }

        ContractEndDate = newEndDate;
        EnsureEmploymentXor();
        Raise(new ContractRenewed(Id, newEndDate));
        return Result.Success();
    }

    public Result ConvertContractToTenure()
    {
        if (ContractEndDate is null)
        {
            return Result.Failure(Error.Conflict("Academic is not currently under a contract."));
        }

        ContractEndDate = null;
        IsTenured = true;
        EnsureEmploymentXor();
        Raise(new ConvertedToTenure(Id));
        return Result.Success();
    }

    public Result ClearEmployment()
    {
        if (!IsTenured && ContractEndDate is null)
        {
            return Result.Success();
        }

        IsTenured = false;
        ContractEndDate = null;
        EnsureEmploymentXor();
        Raise(new EmploymentCleared(Id));
        return Result.Success();
    }

    public Result AssignExtension(Extension extension)
    {
        if (Extension is not null)
        {
            return Result.Failure(Error.Conflict("Academic already has an extension assigned."));
        }

        Extension = extension;
        Raise(new ExtensionAssigned(Id, extension.ExtNr));
        return Result.Success();
    }

    public Result ReleaseExtension()
    {
        if (Extension is null)
        {
            return Result.Failure(Error.Conflict("Academic does not have an extension to release."));
        }

        var released = Extension;
        Extension = null;
        Raise(new ExtensionReleased(Id, released.ExtNr));
        return Result.Success();
    }

    public Result AddQualification(Degree degree, University university)
    {
        if (_qualifications.Any(q => q.Degree.Code == degree.Code))
        {
            return Result.Failure(
                Error.Conflict($"Qualification with degree '{degree.Code}' already exists."));
        }

        _qualifications.Add(AcademicQualification.Create(Id, degree, university));
        Raise(new QualificationAdded(Id, degree.Code, university.Code));
        return Result.Success();
    }

    public Result RemoveQualification(Degree degree)
    {
        var match = _qualifications.FirstOrDefault(q => q.Degree.Code == degree.Code);
        if (match is null)
        {
            return Result.Failure(
                Error.NotFound($"Qualification with degree '{degree.Code}' was not found."));
        }

        if (_qualifications.Count == 1)
        {
            return Result.Failure(
                Error.Conflict("Cannot remove the last qualification; an academic must retain at least one."));
        }

        _qualifications.Remove(match);
        Raise(new QualificationRemoved(Id, degree.Code));
        return Result.Success();
    }

    private void EnsureEmploymentXor()
    {
        if (IsTenured && ContractEndDate is not null)
        {
            throw new BusinessRuleViolationException(
                "Academic cannot be simultaneously tenured and under a fixed-term contract.");
        }
    }
}
