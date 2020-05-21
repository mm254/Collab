using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IvA.Migrations
{
    public partial class ProjekteErstelltAm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ErstelltAm",
                table: "Projekte",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErstelltAm",
                table: "Projekte");
        }
    }
}
