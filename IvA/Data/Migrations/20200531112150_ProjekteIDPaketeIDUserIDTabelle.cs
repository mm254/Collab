using Microsoft.EntityFrameworkCore.Migrations;

namespace IvA.Data.Migrations
{
    public partial class ProjekteIDPaketeIDUserIDTabelle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaketeUserViewModel",
                columns: table => new
                {
                    PaketeUserViewModelId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArbeitsPaketId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaketeUserViewModel", x => x.PaketeUserViewModelId);
                });

            migrationBuilder.CreateTable(
                name: "ProjekteUserViewModel",
                columns: table => new
                {
                    ProjekteUserViewModelId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjekteId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjekteUserViewModel", x => x.ProjekteUserViewModelId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaketeUserViewModel");

            migrationBuilder.DropTable(
                name: "ProjekteUserViewModel");
        }
    }
}
