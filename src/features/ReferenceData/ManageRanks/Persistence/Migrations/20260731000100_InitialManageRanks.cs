using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence.Migrations
{
  public partial class InitialManageRanks : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Ranks",
          columns: table => new
          {
            Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Ranks", x => x.Code);
            table.CheckConstraint("CK_Ranks_Code_Allowed", "[Code] IN ('L', 'P', 'SL')");
          });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Ranks");
    }
  }
}
