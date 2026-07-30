using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Features.SharedKernel.Foundation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSharedKernel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Academics",
                columns: table => new
                {
                    EmpNr = table.Column<string>(type: "nchar(6)", fixedLength: true, maxLength: 6, nullable: false),
                    EmpName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IsTenured = table.Column<bool>(type: "bit", nullable: false),
                    ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academics", x => x.EmpNr);
                    table.CheckConstraint("CK_Academics_EmploymentMutualExclusion", "NOT ([IsTenured] = 1 AND [ContractEndDate] IS NOT NULL)");
                });

            migrationBuilder.CreateTable(
                name: "Degrees",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degrees", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Extensions",
                columns: table => new
                {
                    Number = table.Column<decimal>(type: "decimal(10,0)", precision: 10, scale: 0, nullable: false),
                    AssignedEmpNr = table.Column<string>(type: "nchar(6)", fixedLength: true, maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extensions", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Extensions_Academics_AssignedEmpNr",
                        column: x => x.AssignedEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AcademicQualifications",
                columns: table => new
                {
                    EmpNr = table.Column<string>(type: "nchar(6)", fixedLength: true, maxLength: 6, nullable: false),
                    DegreeCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    UniversityCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicQualifications", x => new { x.EmpNr, x.DegreeCode });
                    table.ForeignKey(
                        name: "FK_AcademicQualifications_Academics_EmpNr",
                        column: x => x.EmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicQualifications_Degrees_DegreeCode",
                        column: x => x.DegreeCode,
                        principalTable: "Degrees",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicQualifications_Universities_UniversityCode",
                        column: x => x.UniversityCode,
                        principalTable: "Universities",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicQualifications_DegreeCode",
                table: "AcademicQualifications",
                column: "DegreeCode");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicQualifications_UniversityCode",
                table: "AcademicQualifications",
                column: "UniversityCode");

            migrationBuilder.CreateIndex(
                name: "UX_Extensions_AssignedEmpNr",
                table: "Extensions",
                column: "AssignedEmpNr",
                unique: true,
                filter: "[AssignedEmpNr] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicQualifications");

            migrationBuilder.DropTable(
                name: "Extensions");

            migrationBuilder.DropTable(
                name: "Degrees");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "Academics");
        }
    }
}
