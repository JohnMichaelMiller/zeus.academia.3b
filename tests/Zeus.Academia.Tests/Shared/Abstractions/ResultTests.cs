using Zeus.Academia.Shared.Abstractions;

namespace Zeus.Academia.Tests.Shared.Abstractions;

public class ResultTests
{
    [Fact]
    public void Success_NonGeneric_Has_No_Error()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Success_Generic_Carries_Value()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_Generic_Throws_On_Value_Access()
    {
        var result = Result<int>.Failure(Error.NotFound("missing"));

        result.IsFailure.Should().BeTrue();
        Action act = () => _ = result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Implicit_Conversion_From_Value_Succeeds()
    {
        Result<string> result = "hello";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void Implicit_Conversion_From_Error_Fails()
    {
        Result<string> result = Error.Conflict("conflict");

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void Error_Factories_Produce_Correct_Types()
    {
        Error.None.Type.Should().Be(ErrorType.None);
        Error.NotFound("x").Type.Should().Be(ErrorType.NotFound);
        Error.Conflict("x").Type.Should().Be(ErrorType.Conflict);
        Error.Validation("x").Type.Should().Be(ErrorType.Validation);
        Error.Failure("code", "x").Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void Failure_NonGeneric_With_None_Error_Throws()
    {
        Action act = () => Result.Failure(Error.None);
        act.Should().Throw<InvalidOperationException>();
    }
}
