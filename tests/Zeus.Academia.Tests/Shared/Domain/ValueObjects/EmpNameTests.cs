using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.ValueObjects;

public class EmpNameTests
{
    [Fact]
    public void Create_Empty_Fails()
    {
        EmpName.Create("").IsFailure.Should().BeTrue();
        EmpName.Create("   ").IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Sixteen_Chars_Fails()
    {
        var result = EmpName.Create(new string('A', 16));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Fifteen_Chars_Succeeds()
    {
        var value = new string('A', 15);

        var result = EmpName.Create(value);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Fact]
    public void Create_Trims_Whitespace()
    {
        var result = EmpName.Create("  Smith  ");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Smith");
    }
}
