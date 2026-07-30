using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;

public sealed class ManageRanksConfiguration : IEntityTypeConfiguration<ManagedRank>
{
  public void Configure(EntityTypeBuilder<ManagedRank> builder)
  {
    builder.ToTable("Ranks", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint("CK_Ranks_Code_Allowed", RankCodeMapping.SqlAllowedCodeConstraint);
    });

    builder.HasKey(x => x.Rank);

    builder.Property(x => x.Rank)
        .HasConversion<string>()
        .HasMaxLength(2)
        .IsRequired();

    builder.Ignore(x => x.Code);
    builder.Ignore(x => x.AccessLevel);
  }
}
