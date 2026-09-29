using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Features.Extensions.ProvisionExtension.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ExtensionAssignedEmpNrLengthCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Extensions_AssignedEmpNrLength",
                table: "Extensions",
                sql: "[AssignedEmpNr] IS NULL OR LEN([AssignedEmpNr]) = 6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Extensions_AssignedEmpNrLength",
                table: "Extensions");
        }
    }
}
