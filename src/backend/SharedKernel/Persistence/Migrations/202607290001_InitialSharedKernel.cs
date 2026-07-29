using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zeus.Academia.Backend.SharedKernel.Persistence.Migrations;

public partial class InitialSharedKernel : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    ArgumentNullException.ThrowIfNull(migrationBuilder);

    migrationBuilder.CreateTable(
      name: "Academics",
      columns: table => new
      {
        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
        EmpNr = table.Column<string>(type: "nchar(6)", fixedLength: true, maxLength: 6, nullable: false),
        EmpName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
        RankCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
        IsTenured = table.Column<bool>(type: "bit", nullable: false),
        ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true),
        ExtensionNumber = table.Column<int>(type: "int", nullable: false)
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_Academics", x => x.Id);
        table.CheckConstraint("CK_Academics_TenureContract_Exclusive", "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
      });

    migrationBuilder.CreateTable(
      name: "AcademicQualifications",
      columns: table => new
      {
        AcademicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
        DegreeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
        UniversityCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_AcademicQualifications", x => new { x.AcademicId, x.DegreeCode });
        table.ForeignKey(
          name: "FK_AcademicQualifications_Academics_AcademicId",
          column: x => x.AcademicId,
          principalTable: "Academics",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
      });

    migrationBuilder.CreateIndex(
      name: "IX_Academics_EmpNr",
      table: "Academics",
      column: "EmpNr",
      unique: true);

    migrationBuilder.CreateIndex(
      name: "IX_Academics_ExtensionNumber",
      table: "Academics",
      column: "ExtensionNumber",
      unique: true);
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    ArgumentNullException.ThrowIfNull(migrationBuilder);

    migrationBuilder.DropTable(
      name: "AcademicQualifications");

    migrationBuilder.DropTable(
      name: "Academics");
  }
}
