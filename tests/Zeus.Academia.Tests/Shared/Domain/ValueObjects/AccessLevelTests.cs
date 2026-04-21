using System.Reflection;
using Zeus.Academia.Shared.Domain.ValueObjects;

namespace Zeus.Academia.Tests.Shared.Domain.ValueObjects;

public class AccessLevelTests
{
    [Fact]
    public void FromRank_P_Maps_To_INT()
    {
        AccessLevel.FromRank(Rank.P).Should().Be(AccessLevel.INT);
    }

    [Fact]
    public void FromRank_SL_Maps_To_NAT()
    {
        AccessLevel.FromRank(Rank.SL).Should().Be(AccessLevel.NAT);
    }

    [Fact]
    public void FromRank_L_Maps_To_LOC()
    {
        AccessLevel.FromRank(Rank.L).Should().Be(AccessLevel.LOC);
    }

    [Fact]
    public void No_Public_String_Based_Create_Factory_Exists()
    {
        var createFromString = typeof(AccessLevel).GetMethods(
                BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "Create")
            .Any(m =>
            {
                var parameters = m.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
            });

        createFromString.Should().BeFalse(
            "AccessLevel must only be created via FromRank to preserve the derivation rule.");
    }
}
