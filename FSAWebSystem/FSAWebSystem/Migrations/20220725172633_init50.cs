using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init50 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleAccesses_RoleUnilevers_RoleUnileverId",
                table: "RoleAccesses");

            migrationBuilder.DropIndex(
                name: "IX_RoleAccesses_RoleUnileverId",
                table: "RoleAccesses");

            migrationBuilder.DropColumn(
                name: "AccessActivity",
                table: "RoleAccesses");

            migrationBuilder.DropColumn(
                name: "Menu",
                table: "RoleAccesses");

            migrationBuilder.DropColumn(
                name: "SubMenu",
                table: "RoleAccesses");

            migrationBuilder.RenameColumn(
                name: "RoleUnileverId",
                table: "RoleAccesses",
                newName: "RoleId");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleAccessId",
                table: "RoleUnilevers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_RoleAccesses_RoleAccessId",
                        column: x => x.RoleAccessId,
                        principalTable: "RoleAccesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleUnilevers_RoleAccessId",
                table: "RoleUnilevers",
                column: "RoleAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_RoleAccessId",
                table: "Menus",
                column: "RoleAccessId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUnilevers_RoleAccesses_RoleAccessId",
                table: "RoleUnilevers",
                column: "RoleAccessId",
                principalTable: "RoleAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUnilevers_RoleAccesses_RoleAccessId",
                table: "RoleUnilevers");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_RoleUnilevers_RoleAccessId",
                table: "RoleUnilevers");

            migrationBuilder.DropColumn(
                name: "RoleAccessId",
                table: "RoleUnilevers");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "RoleAccesses",
                newName: "RoleUnileverId");

            migrationBuilder.AddColumn<string>(
                name: "AccessActivity",
                table: "RoleAccesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Menu",
                table: "RoleAccesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubMenu",
                table: "RoleAccesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_RoleUnileverId",
                table: "RoleAccesses",
                column: "RoleUnileverId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleAccesses_RoleUnilevers_RoleUnileverId",
                table: "RoleAccesses",
                column: "RoleUnileverId",
                principalTable: "RoleUnilevers",
                principalColumn: "RoleUnileverId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
