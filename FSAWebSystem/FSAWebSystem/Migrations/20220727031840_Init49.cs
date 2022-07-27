using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init49 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleAccesses");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RoleUnilevers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RoleUnilevers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "RoleUnilevers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "RoleUnilevers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

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

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RoleUnilevers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoleUnilevers");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "RoleUnilevers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "RoleUnilevers");

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleUnileverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessActivity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Menu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubMenu = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                column: "RoleUnileverId");
        }
    }
}
