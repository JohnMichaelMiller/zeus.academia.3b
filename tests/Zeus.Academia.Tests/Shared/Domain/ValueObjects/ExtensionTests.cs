using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.ValueObjects;

public class ExtensionTests
{
    [Theory]
    [InlineData("1234")]
    [InlineData("123")]
    [InlineData("123456")]
    public void Create_With_Valid_Digits_Succeeds(string input)
    {
        var result = Extension.Create(input);

        result.IsSuccess.Should().BeTrue();
        result.Value.ExtNr.Should().Be(input);
    }

    [Theory]
    [InlineData("12")]
    [InlineData("12A")]
    [InlineData("1234567")]
    [InlineData("")]
    public void Create_With_Invalid_Input_Fails(string input)
    {
        var result = Extension.Create(input);

        result.IsFailure.Should().BeTrue();
    }
}
