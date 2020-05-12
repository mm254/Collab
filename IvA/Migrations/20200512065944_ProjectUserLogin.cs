using Microsoft.EntityFrameworkCore.Migrations;

// Aufbau der Datenbank per EF Core Migration

namespace IvA.Migrations
{
    public partial class ProjectUserLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectUserLoginContexts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    References = table.Column<string>(nullable: true),
                    EnterpriseName = table.Column<string>(nullable: true),
                    Mail = table.Column<string>(nullable: true),
                    UserPassword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUserLoginContexts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectUserLoginContexts");
        }
    }
}
