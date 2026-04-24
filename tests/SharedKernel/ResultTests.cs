namespace Zeus.Academia.SharedKernel.Tests;

using FluentAssertions;
using Xunit;
using Zeus.Academia.SharedKernel.Results;

public sealed class ResultTests
{
    [Fact]
    public void Success_CarriesValue()
    {
        var r = Result<int>.Success(42);

        r.IsSuccess.Should().BeTrue();
        r.IsFailure.Should().BeFalse();
        r.Value.Should().Be(42);
        r.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_CarriesError_AndAccessingValueThrows()
    {
        var err = new Error("Test.Oops", "Something broke.");
        var r = Result<int>.Failure(err);

        r.IsFailure.Should().BeTrue();
        r.Error.Should().Be(err);

        var act = () => r.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NonGenericResult_Success_And_Failure()
    {
        Result.Success().IsSuccess.Should().BeTrue();
        Result.Failure(new Error("X", "Y")).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Success_WithError_Throws()
    {
        var act = () => Result.Failure(Error.None);
        act.Should().Throw<InvalidOperationException>();
    }
}
