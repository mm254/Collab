using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace IvA.Data.Migrations
{
    public partial class UserMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    MessageID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuellID = table.Column<int>(nullable: false),
                    ZielID = table.Column<int>(nullable: false),
                    Nachricht = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Datum = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.MessageID);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    UserLoginID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => x.UserLoginID);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginMessageViewModel",
                columns: table => new
                {
                    UserLoginMessageViewModelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserLoginID = table.Column<int>(nullable: false),
                    MessageID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginMessageViewModel", x => x.UserLoginMessageViewModelID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserLoginMessageViewModel");
        }
    }
}
