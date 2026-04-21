using Zeus.Academia.Shared.Domain.Academics.Events;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.Academics;

public class AcademicRankTests
{
    [Fact]
    public void ChangeRank_FromPToSL_UpdatesRankAndRecomputesAccessLevel()
    {
        var academic = AcademicTestBuilder.RegisterDefault(Rank.P);
        academic.AccessLevel.Should().Be(AccessLevel.INT);

        var result = academic.ChangeRank(Rank.SL);

        result.IsSuccess.Should().BeTrue();
        academic.Rank.Should().Be(Rank.SL);
        academic.AccessLevel.Should().Be(AccessLevel.NAT);

        var evt = academic.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RankChanged>().Subject;
        evt.OldRank.Should().Be("P");
        evt.NewRank.Should().Be("SL");
        evt.OldAccessLevel.Should().Be("INT");
        evt.NewAccessLevel.Should().Be("NAT");
    }
}
