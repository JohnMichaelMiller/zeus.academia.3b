using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.ValueObjects;

public class EmpNrTests
{
    [Fact]
    public void Create_With_Six_Chars_Succeeds()
    {
        var result = EmpNr.Create("ABC123");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("ABC123");
    }

    [Theory]
    [InlineData("ABC12")]
    [InlineData("ABC1234")]
    public void Create_With_Wrong_Length_Fails(string value)
    {
        var result = EmpNr.Create(value);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_With_Whitespace_Fails()
    {
        var result = EmpNr.Create("AB C12");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Equality_By_Value()
    {
        var a = EmpNr.Create("ABC123").Value;
        var b = EmpNr.Create("ABC123").Value;

        a.Should().Be(b);
    }
}
