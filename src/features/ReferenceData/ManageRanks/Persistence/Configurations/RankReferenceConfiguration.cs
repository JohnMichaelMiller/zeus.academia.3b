using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence.Configurations;

public sealed class RankReferenceConfiguration : IEntityTypeConfiguration<RankReference>
{
  public void Configure(EntityTypeBuilder<RankReference> builder)
  {
    builder.ToTable("Ranks", tableBuilder =>
    {
      tableBuilder.HasCheckConstraint(
          "CK_Ranks_Code_Allowed",
          RankCatalog.BuildAllowedCodeCheckConstraintSql(nameof(RankReference.Code)));
    });

    builder.HasKey(x => x.Code);

    builder.Property(x => x.Code)
        .HasMaxLength(SharedKernelFieldLengths.Code)
        .IsRequired();
  }
}
