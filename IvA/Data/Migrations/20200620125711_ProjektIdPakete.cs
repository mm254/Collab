using Microsoft.EntityFrameworkCore.Migrations;

namespace IvA.Data.Migrations
{
    public partial class ProjektIdPakete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjektId",
                table: "ArbeitsPaket",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjektId",
                table: "ArbeitsPaket");
        }
    }
}
