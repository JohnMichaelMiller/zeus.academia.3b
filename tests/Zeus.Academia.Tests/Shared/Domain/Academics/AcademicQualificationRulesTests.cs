using Zeus.Academia.Shared.Abstractions;
using Zeus.Academia.Shared.Domain.Academics.Events;

namespace Zeus.Academia.Tests.Shared.Domain.Academics;

public class AcademicQualificationRulesTests
{
    [Fact]
    public void AddQualification_DuplicateDegreeCode_FailsConflict()
    {
        var academic = AcademicTestBuilder.RegisterDefault(degreeCode: "BSC");

        var result = academic.AddQualification(
            AcademicTestBuilder.Degree("BSC"),
            AcademicTestBuilder.University("STAN"));

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void AddQualification_HappyPath_AddsAndRaisesEvent()
    {
        var academic = AcademicTestBuilder.RegisterDefault(degreeCode: "BSC");

        var result = academic.AddQualification(
            AcademicTestBuilder.Degree("MSC"),
            AcademicTestBuilder.University("STAN"));

        result.IsSuccess.Should().BeTrue();
        academic.Qualifications.Should().HaveCount(2);
        var evt = academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<QualificationAdded>().Subject;
        evt.DegreeCode.Should().Be("MSC");
        evt.UniversityCode.Should().Be("STAN");
    }

    [Fact]
    public void RemoveQualification_UnknownDegree_FailsNotFound()
    {
        var academic = AcademicTestBuilder.RegisterDefault(degreeCode: "BSC");

        var result = academic.RemoveQualification(AcademicTestBuilder.Degree("PHD"));

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void RemoveQualification_WhenOnlyOne_FailsConflict()
    {
        var academic = AcademicTestBuilder.RegisterDefault(degreeCode: "BSC");

        var result = academic.RemoveQualification(AcademicTestBuilder.Degree("BSC"));

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        academic.Qualifications.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveQualification_HappyPath_RemovesAndRaisesEvent()
    {
        var academic = AcademicTestBuilder.RegisterDefault(degreeCode: "BSC");
        academic.AddQualification(
            AcademicTestBuilder.Degree("MSC"),
            AcademicTestBuilder.University("STAN")).IsSuccess.Should().BeTrue();
        academic.ClearDomainEvents();

        var result = academic.RemoveQualification(AcademicTestBuilder.Degree("MSC"));

        result.IsSuccess.Should().BeTrue();
        academic.Qualifications.Should().HaveCount(1);
        academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<QualificationRemoved>()
            .Which.DegreeCode.Should().Be("MSC");
    }
}
