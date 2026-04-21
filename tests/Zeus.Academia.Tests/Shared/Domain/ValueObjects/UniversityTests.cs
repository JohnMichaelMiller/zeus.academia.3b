using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.ValueObjects;

public class UniversityTests
{
    [Fact]
    public void Create_Empty_Fails()
    {
        University.Create("").IsFailure.Should().BeTrue();
        University.Create("   ").IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Upper_Cases_Value()
    {
        var result = University.Create("mit");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("MIT");
    }

    [Fact]
    public void Create_Over_Max_Length_Fails()
    {
        var result = University.Create(new string('A', 11));

        result.IsFailure.Should().BeTrue();
    }
}
