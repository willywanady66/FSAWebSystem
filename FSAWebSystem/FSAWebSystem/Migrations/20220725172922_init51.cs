using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init51 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleUnilevers_RoleAccesses_RoleAccessId",
                table: "RoleUnilevers");

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

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_RoleUnileverId",
                table: "RoleAccesses",
                column: "RoleUnileverId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleAccesses_RoleUnilevers_RoleUnileverId",
                table: "RoleAccesses",
                column: "RoleUnileverId",
                principalTable: "RoleUnilevers",
                principalColumn: "RoleUnileverId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleAccesses_RoleUnilevers_RoleUnileverId",
                table: "RoleAccesses");

            migrationBuilder.DropIndex(
                name: "IX_RoleAccesses_RoleUnileverId",
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

            migrationBuilder.CreateIndex(
                name: "IX_RoleUnilevers_RoleAccessId",
                table: "RoleUnilevers",
                column: "RoleAccessId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUnilevers_RoleAccesses_RoleAccessId",
                table: "RoleUnilevers",
                column: "RoleAccessId",
                principalTable: "RoleAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
