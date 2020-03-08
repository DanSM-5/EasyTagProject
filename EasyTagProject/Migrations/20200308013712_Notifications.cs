using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyTagProject.Migrations
{
    public partial class Notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_End_RoomId",
                table: "Appointments",
                columns: new[] { "End", "RoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Date_RoomId",
                table: "Notifications",
                columns: new[] { "Date", "RoomId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_End_RoomId",
                table: "Appointments");
        }
    }
}
