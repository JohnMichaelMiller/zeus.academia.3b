using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation.Domain;

public sealed class ResultTests
{
    [Fact]
    public void Success_ReturnsValueAndMarksResultSuccessful()
    {
        var result = Result<string>.Success("ok");

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("ok", result.Value);
    }

    [Fact]
    public void Failure_ReturnsErrorAndMarksResultFailed()
    {
        var error = Error.Create("shared.error", "Something went wrong.");

        var result = Result<string>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }
}