using Microsoft.EntityFrameworkCore.Migrations;

namespace IvA.Data.Migrations
{
    public partial class Zeitbedarf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VerbrauchteZeit",
                table: "ArbeitsPaket",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Zeitbudget",
                table: "ArbeitsPaket",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerbrauchteZeit",
                table: "ArbeitsPaket");

            migrationBuilder.DropColumn(
                name: "Zeitbudget",
                table: "ArbeitsPaket");
        }
    }
}
