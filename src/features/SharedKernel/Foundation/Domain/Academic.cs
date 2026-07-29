using Zeus.Academia.Features.SharedKernel.Foundation.Domain.Exceptions;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain.ValueObjects;

namespace Zeus.Academia.Features.SharedKernel.Foundation.Domain;

public sealed class Academic
{
    private decimal? _extensionNumber;

    private Academic()
    {
        EmpName = string.Empty;
    }

    private Academic(EmpNr empNr, string empName, Rank rank, bool isTenured, DateOnly? contractEndDate)
    {
        EmpNr = empNr;
        EmpName = NormalizeName(empName);
        Rank = ValidateRank(rank);

        if (isTenured && contractEndDate is not null)
        {
            throw new BusinessRuleViolationException("Academic cannot be tenured and contracted at the same time.");
        }

        IsTenured = isTenured;
        ContractEndDate = contractEndDate;
    }

    public EmpNr EmpNr { get; private set; }

    public string EmpName { get; private set; }

    public Rank Rank { get; private set; }

    public AccessLevel AccessLevel => Rank.ToAccessLevel();

    public bool IsTenured { get; private set; }

    public DateOnly? ContractEndDate { get; private set; }

    public Extension? Extension => _extensionNumber.HasValue ? new Extension(_extensionNumber.Value) : null;

    public static Academic Create(EmpNr empNr, string empName, Rank rank, bool isTenured = false, DateOnly? contractEndDate = null)
        => new(empNr, empName, rank, isTenured, contractEndDate);

    public void UpdateName(string empName)
    {
        EmpName = NormalizeName(empName);
    }

    public void GrantTenure()
    {
        IsTenured = true;
        ContractEndDate = null;
    }

    public void AssignContract(DateOnly contractEndDate)
    {
        if (contractEndDate <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BusinessRuleViolationException("Contract end date must be in the future.");
        }

        IsTenured = false;
        ContractEndDate = contractEndDate;
    }

    public void RenewContract(DateOnly contractEndDate)
    {
        if (ContractEndDate is null)
        {
            throw new BusinessRuleViolationException("Academic must already be contracted before renewals are allowed.");
        }

        AssignContract(contractEndDate);
    }

    public void ConvertContractToTenure()
    {
        if (ContractEndDate is null)
        {
            throw new BusinessRuleViolationException("Academic must be contracted before conversion to tenure.");
        }

        GrantTenure();
    }

    public void RemoveEmploymentStatus()
    {
        IsTenured = false;
        ContractEndDate = null;
    }

    public void ChangeRank(Rank rank)
    {
        Rank = ValidateRank(rank);
    }

    public void AssignExtension(Extension extension)
    {
        _extensionNumber = extension.Value;
    }

    public void ReleaseExtension()
    {
        _extensionNumber = null;
    }

    private static string NormalizeName(string empName)
    {
        if (string.IsNullOrWhiteSpace(empName))
        {
            throw new ArgumentException("Employee name must not be empty.", nameof(empName));
        }

        var normalized = empName.Trim();
        if (normalized.Length > 15)
        {
            throw new ArgumentException("Employee name must not exceed 15 characters.", nameof(empName));
        }

        return normalized;
    }

    private static Rank ValidateRank(Rank rank)
    {
        if (!Enum.IsDefined(rank))
        {
            throw new ArgumentOutOfRangeException(nameof(rank), rank, "Unsupported rank.");
        }

        return rank;
    }
}