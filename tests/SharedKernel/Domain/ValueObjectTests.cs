using FluentAssertions;
using Zeus.Academia.SharedKernel.Domain.ValueObjects;
using Zeus.Academia.SharedKernel.Exceptions;

namespace Zeus.Academia.SharedKernel.Tests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void EmpNr_MustBeSixCharacters()
    {
        EmpNr.Create("ABC123").Value.Should().Be("ABC123");

        Action tooShort = () => EmpNr.Create("ABC");
        Action tooLong = () => EmpNr.Create("ABCDEFG");

        tooShort.Should().Throw<BusinessRuleViolationException>();
        tooLong.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Extension_MustBePositive()
    {
        Extension.Create(101.5m).ExtNr.Should().Be(101.5m);

        Action zero = () => Extension.Create(0m);
        Action negative = () => Extension.Create(-1m);

        zero.Should().Throw<BusinessRuleViolationException>();
        negative.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Degree_RequiresNonEmptyCode()
    {
        Degree.Create("PHD").Code.Should().Be("PHD");
        Action empty = () => Degree.Create("   ");
        empty.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void University_RequiresNonEmptyCode()
    {
        University.Create("MIT").Code.Should().Be("MIT");
        Action empty = () => University.Create("");
        empty.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void AcademicQualification_CombinesDegreeAndUniversity()
    {
        var qual = AcademicQualification.Create(
            "EMP001",
            Degree.Create("PHD"),
            University.Create("MIT"));

        qual.AcademicEmpNr.Should().Be("EMP001");
        qual.Degree.Code.Should().Be("PHD");
        qual.University.Code.Should().Be("MIT");
    }

    [Fact]
    public void AccessLevelDerivation_KnownMappings()
    {
        AccessLevelDerivation.From(Rank.P).Should().Be(AccessLevel.INT);
        AccessLevelDerivation.From(Rank.SL).Should().Be(AccessLevel.NAT);
        AccessLevelDerivation.From(Rank.L).Should().Be(AccessLevel.LOC);
    }

    [Fact]
    public void AccessLevelDerivation_UnknownRank_Throws()
    {
        Action act = () => AccessLevelDerivation.From((Rank)99);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
