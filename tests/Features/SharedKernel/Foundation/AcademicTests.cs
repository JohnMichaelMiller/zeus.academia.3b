using Zeus.Academia.Features.SharedKernel.Foundation.Entities;
using Zeus.Academia.Features.SharedKernel.Foundation.ValueObjects;
using Xunit;

namespace Zeus.Academia.Tests.Features.SharedKernel.Foundation;

public sealed class AcademicTests
{
  [Fact]
  public void Create_WithRank_SetsDerivedAccessLevel()
  {
    var academic = Academic.Create("ABC123", "Ada Lovelace", Rank.Create("SL"));

    Assert.Equal("SL", academic.Rank.Code);
    Assert.Equal("NAT", academic.AccessLevel.Code);
  }

  [Fact]
  public void ChangeRank_UpdatesDerivedAccessLevel()
  {
    var academic = Academic.Create("ABC123", "Ada Lovelace", Rank.Create("P"));

    academic.ChangeRank(Rank.Create("L"));

    Assert.Equal("L", academic.Rank.Code);
    Assert.Equal("LOC", academic.AccessLevel.Code);
  }

  [Fact]
  public void GrantTenure_AndAssignContract_ClearTheOppositeState()
  {
    var academic = Academic.Create("ABC123", "Ada Lovelace", Rank.Create("P"));

    academic.AssignContract(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));
    Assert.False(academic.IsTenured);
    Assert.NotNull(academic.ContractEndDate);

    academic.GrantTenure();

    Assert.True(academic.IsTenured);
    Assert.Null(academic.ContractEndDate);
  }

  [Fact]
  public void AssignExtension_WithDifferentExistingExtension_Throws()
  {
    var academic = Academic.Create("ABC123", "Ada Lovelace", Rank.Create("P"), Extension.Create("1001"));

    var exception = Assert.Throws<Zeus.Academia.Features.SharedKernel.Foundation.Exceptions.ExtensionAssignmentConflictException>(
        () => academic.AssignExtension(Extension.Create("2002")));

    Assert.Contains("already owns extension", exception.Message);
  }

  [Fact]
  public void ReleaseExtension_WithDifferentExtension_Throws()
  {
    var academic = Academic.Create("ABC123", "Ada Lovelace", Rank.Create("P"), Extension.Create("1001"));

    var exception = Assert.Throws<Zeus.Academia.Features.SharedKernel.Foundation.Exceptions.ExtensionOwnershipMismatchException>(
        () => academic.ReleaseExtension(Extension.Create("2002")));

    Assert.Contains("owns 1001", exception.Message);
  }
}
