namespace Zeus.Academia.SharedKernel.Tests;

using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Domain.Entities;

public sealed class AcademicQualificationTests
{
    [Fact]
    public void Create_ValidatesInputs_AndNormalizesCodes()
    {
        var result = AcademicQualification.Create("A00001", "phd", "ucsd");

        result.IsSuccess.Should().BeTrue();
        var q = result.Value;
        q.AcademicEmpNr.Should().Be("A00001");
        q.DegreeCode.Should().Be("PHD");
        q.UniversityCode.Should().Be("UCSD");
    }

    [Fact]
    public void Equality_ByAcademicAndDegreeOnly()
    {
        var a = AcademicQualification.Create("A00001", "PHD", "UCSD").Value;
        var b = AcademicQualification.Create("A00001", "PHD", "MIT").Value;
        var c = AcademicQualification.Create("A00001", "MCS", "UCSD").Value;

        a.Should().Be(b);
        a.Should().NotBe(c);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Theory]
    [InlineData("", "PHD", "UCSD", "Qualification.EmpNr.Empty")]
    [InlineData("A00001", "", "UCSD", "Qualification.Degree.Empty")]
    [InlineData("A00001", "PHD", "", "Qualification.University.Empty")]
    public void Create_RejectsMissingFields(string emp, string degree, string university, string code)
    {
        var result = AcademicQualification.Create(emp, degree, university);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(code);
    }
}
