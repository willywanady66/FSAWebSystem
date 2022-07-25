using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init52 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_RoleAccesses_RoleAccessId",
                table: "Menus");

            migrationBuilder.DropTable(
                name: "RoleAccesses");

            migrationBuilder.RenameColumn(
                name: "RoleAccessId",
                table: "Menus",
                newName: "RoleUnileverId");

            migrationBuilder.RenameIndex(
                name: "IX_Menus_RoleAccessId",
                table: "Menus",
                newName: "IX_Menus_RoleUnileverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_RoleUnilevers_RoleUnileverId",
                table: "Menus",
                column: "RoleUnileverId",
                principalTable: "RoleUnilevers",
                principalColumn: "RoleUnileverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_RoleUnilevers_RoleUnileverId",
                table: "Menus");

            migrationBuilder.RenameColumn(
                name: "RoleUnileverId",
                table: "Menus",
                newName: "RoleAccessId");

            migrationBuilder.RenameIndex(
                name: "IX_Menus_RoleUnileverId",
                table: "Menus",
                newName: "IX_Menus_RoleAccessId");

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccesses", x => x.Id);
                    table.ForeignKey(

                        name: "FK_RoleAccesses_RoleUnilevers_RoleUnileverId",
                        column: x => x.RoleUnileverId,
                        principalTable: "RoleUnilevers",
                        principalColumn: "RoleUnileverId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_RoleUnileverId",
                table: "RoleAccesses",
                column: "RoleUnileverId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_RoleAccesses_RoleAccessId",
                table: "Menus",
                column: "RoleAccessId",
                principalTable: "RoleAccesses",
                principalColumn: "Id");
        }
    }
}
