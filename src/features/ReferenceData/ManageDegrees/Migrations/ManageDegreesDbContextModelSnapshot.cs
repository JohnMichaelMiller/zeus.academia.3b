using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageDegrees.Migrations;

[DbContext(typeof(ManageDegreesDbContext))]
public partial class ManageDegreesDbContextModelSnapshot : ModelSnapshot
{
  protected override void BuildModel(ModelBuilder modelBuilder)
  {
    modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

    modelBuilder.Entity("Zeus.Academia.Features.ReferenceData.ManageDegrees.Shared.DegreeRecord", b =>
    {
      b.Property<string>("Code")
        .HasMaxLength(SharedKernelFieldLengths.DegreeCode)
        .IsRequired();

      b.HasKey("Code");

      b.ToTable("Degrees");
    });
  }
}
