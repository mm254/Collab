using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IvA.Migrations
{
    public partial class Projekte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projekte",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Projektname = table.Column<string>(nullable: true),
                    Projektersteller = table.Column<string>(nullable: true),
                    Mitglieder = table.Column<string>(nullable: true),
                    Beschreibung = table.Column<string>(nullable: true),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projekte", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projekte");
        }
    }
}
