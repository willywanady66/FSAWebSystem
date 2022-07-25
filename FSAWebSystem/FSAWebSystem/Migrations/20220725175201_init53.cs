using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init53 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_RoleUnilevers_RoleUnileverId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_RoleUnileverId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "RoleUnileverId",
                table: "Menus");

            migrationBuilder.CreateTable(
                name: "MenuRoleUnilever",
                columns: table => new
                {
                    MenusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleUnileversRoleUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRoleUnilever", x => new { x.MenusId, x.RoleUnileversRoleUnileverId });
                    table.ForeignKey(
                        name: "FK_MenuRoleUnilever_Menus_MenusId",
                        column: x => x.MenusId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuRoleUnilever_RoleUnilevers_RoleUnileversRoleUnileverId",
                        column: x => x.RoleUnileversRoleUnileverId,
                        principalTable: "RoleUnilevers",
                        principalColumn: "RoleUnileverId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuRoleUnilever_RoleUnileversRoleUnileverId",
                table: "MenuRoleUnilever",
                column: "RoleUnileversRoleUnileverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuRoleUnilever");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleUnileverId",
                table: "Menus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_RoleUnileverId",
                table: "Menus",
                column: "RoleUnileverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_RoleUnilevers_RoleUnileverId",
                table: "Menus",
                column: "RoleUnileverId",
                principalTable: "RoleUnilevers",
                principalColumn: "RoleUnileverId");
        }
    }
}
