using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Extensions",
                columns: table => new
                {
                    ExtNr = table.Column<decimal>(type: "decimal(10,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extensions", x => x.ExtNr);
                });

            migrationBuilder.CreateTable(
                name: "Academics",
                columns: table => new
                {
                    EmpNr = table.Column<string>(type: "char(6)", fixedLength: true, maxLength: 6, nullable: false),
                    EmpName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IsTenured = table.Column<bool>(type: "bit", nullable: true),
                    ContractEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExtensionExtNr = table.Column<decimal>(type: "decimal(10,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academics", x => x.EmpNr);
                    table.CheckConstraint("CK_Academics_XOR_IsTenured_ContractEndDate", "IsTenured IS NULL OR ContractEndDate IS NULL");
                    table.ForeignKey(
                        name: "FK_Academics_Extensions_ExtensionExtNr",
                        column: x => x.ExtensionExtNr,
                        principalTable: "Extensions",
                        principalColumn: "ExtNr");
                });

            migrationBuilder.CreateTable(
                name: "AcademicQualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<string>(type: "char(6)", fixedLength: true, maxLength: 6, nullable: false),
                    DegreeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UniversityCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicQualifications_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UX_AcademicQualifications_EmpNr_Degree",
                table: "AcademicQualifications",
                columns: new[] { "AcademicEmpNr", "DegreeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Academics_ExtensionExtNr",
                table: "Academics",
                column: "ExtensionExtNr",
                unique: true,
                filter: "[ExtensionExtNr] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicQualifications");

            migrationBuilder.DropTable(
                name: "Academics");

            migrationBuilder.DropTable(
                name: "Extensions");
        }
    }
}
