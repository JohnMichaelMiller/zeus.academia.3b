using Zeus.Academia.Shared.Abstractions;
using Zeus.Academia.Shared.Domain.Academics;
using Zeus.Academia.Shared.Domain.Academics.Events;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.Academics;

public class AcademicRegistrationTests
{
    [Fact]
    public void Register_WithZeroQualifications_FailsValidation()
    {
        var result = Academic.Register(
            AcademicTestBuilder.EmpNr(),
            AcademicTestBuilder.EmpName(),
            Rank.P,
            Array.Empty<(Degree, University)>());

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Register_WithNullQualifications_FailsValidation()
    {
        var result = Academic.Register(
            AcademicTestBuilder.EmpNr(),
            AcademicTestBuilder.EmpName(),
            Rank.P,
            null!);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Register_WithDuplicateDegreeCodes_FailsValidation()
    {
        var qualifications = new[]
        {
            (AcademicTestBuilder.Degree("BSC"), AcademicTestBuilder.University("MIT")),
            (AcademicTestBuilder.Degree("BSC"), AcademicTestBuilder.University("STAN")),
        };

        var result = Academic.Register(
            AcademicTestBuilder.EmpNr(),
            AcademicTestBuilder.EmpName(),
            Rank.P,
            qualifications);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Register_HappyPath_RaisesAcademicRegisteredAndDerivesAccessLevel()
    {
        var qualifications = new[]
        {
            (AcademicTestBuilder.Degree("BSC"), AcademicTestBuilder.University("MIT")),
            (AcademicTestBuilder.Degree("MSC"), AcademicTestBuilder.University("STAN")),
        };

        var result = Academic.Register(
            AcademicTestBuilder.EmpNr("EMP007"),
            AcademicTestBuilder.EmpName("Bob"),
            Rank.P,
            qualifications);

        result.IsSuccess.Should().BeTrue();
        var academic = result.Value;

        academic.AccessLevel.Should().Be(AccessLevel.INT);
        academic.Rank.Should().Be(Rank.P);
        academic.IsTenured.Should().BeFalse();
        academic.ContractEndDate.Should().BeNull();
        academic.Extension.Should().BeNull();
        academic.Qualifications.Should().HaveCount(2);

        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AcademicRegistered>()
            .Which.RankCode.Should().Be("P");
    }

    [Fact]
    public void Register_WithExtension_AssignsExtension()
    {
        var qualifications = new[] { (AcademicTestBuilder.Degree(), AcademicTestBuilder.University()) };
        var extension = AcademicTestBuilder.Extension("5678");

        var result = Academic.Register(
            AcademicTestBuilder.EmpNr(),
            AcademicTestBuilder.EmpName(),
            Rank.L,
            qualifications,
            extension);

        result.IsSuccess.Should().BeTrue();
        result.Value.Extension.Should().Be(extension);
        result.Value.AccessLevel.Should().Be(AccessLevel.LOC);
    }
}
