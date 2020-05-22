using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IvA.Migrations.ArbeitsPaket
{
    public partial class ArbeitsPaketTabelle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbeitsPaket",
                columns: table => new
                {
                    ArbeitsPaketId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaketName = table.Column<string>(nullable: true),
                    Beschreibung = table.Column<string>(nullable: true),
                    Mitglieder = table.Column<string>(nullable: true),
                    Frist = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbeitsPaket", x => x.ArbeitsPaketId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbeitsPaket");
        }
    }
}
