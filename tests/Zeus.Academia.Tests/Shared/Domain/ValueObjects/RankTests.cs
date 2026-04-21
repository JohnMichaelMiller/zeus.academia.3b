using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.ValueObjects;

public class RankTests
{
    [Theory]
    [InlineData("P", "P")]
    [InlineData("sl", "SL")]
    [InlineData("L", "L")]
    [InlineData("p", "P")]
    public void Create_Accepts_Allowed_Codes_Case_Insensitive(string input, string expected)
    {
        var result = Rank.Create(input);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(expected);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("PP")]
    [InlineData("")]
    public void Create_Rejects_Invalid_Codes(string input)
    {
        var result = Rank.Create(input);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Equality_By_Code()
    {
        Rank.Create("P").Value.Should().Be(Rank.P);
        Rank.Create("sl").Value.Should().Be(Rank.SL);
    }
}
