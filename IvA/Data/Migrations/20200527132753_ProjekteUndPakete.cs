using Microsoft.EntityFrameworkCore.Migrations;

namespace IvA.Data.Migrations
{
    public partial class ProjekteUndPakete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjekteArbeitsPaketeViewModelId",
                table: "ProjekteArbeitsPaketeViewModel");

            migrationBuilder.AddColumn<int>(
                name: "ArbeitsPaketId",
                table: "ProjekteArbeitsPaketeViewModel",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArbeitsPaketId",
                table: "ProjekteArbeitsPaketeViewModel");

            migrationBuilder.AddColumn<int>(
                name: "ProjekteArbeitsPaketeViewModelId",
                table: "ProjekteArbeitsPaketeViewModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
