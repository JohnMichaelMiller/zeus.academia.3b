using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.ValueObjects;

public class DegreeTests
{
    [Fact]
    public void Create_Empty_Fails()
    {
        Degree.Create("").IsFailure.Should().BeTrue();
        Degree.Create("  ").IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Upper_Cases_Value()
    {
        var result = Degree.Create("phd");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("PHD");
    }

    [Fact]
    public void Create_Over_Max_Length_Fails()
    {
        var result = Degree.Create(new string('A', 11));

        result.IsFailure.Should().BeTrue();
    }
}
