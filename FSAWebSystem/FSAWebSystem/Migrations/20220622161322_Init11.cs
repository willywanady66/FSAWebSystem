using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleAccesses_RoleUnilevers_RoleId",
                table: "RoleAccesses");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RoleUnilevers",
                newName: "RoleUnileverId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "RoleAccesses",
                newName: "RoleUnileverId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleAccesses_RoleId",
                table: "RoleAccesses",
                newName: "IX_RoleAccesses_RoleUnileverId");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleUnileverId",
                table: "UsersUnilever",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UsersUnilever_RoleUnileverId",
                table: "UsersUnilever",
                column: "RoleUnileverId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleAccesses_RoleUnilevers_RoleUnileverId",
                table: "RoleAccesses",
                column: "RoleUnileverId",
                principalTable: "RoleUnilevers",
                principalColumn: "RoleUnileverId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersUnilever_RoleUnilevers_RoleUnileverId",
                table: "UsersUnilever",
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

            migrationBuilder.DropForeignKey(
                name: "FK_UsersUnilever_RoleUnilevers_RoleUnileverId",
                table: "UsersUnilever");

            migrationBuilder.DropIndex(
                name: "IX_UsersUnilever_RoleUnileverId",
                table: "UsersUnilever");

            migrationBuilder.DropColumn(
                name: "RoleUnileverId",
                table: "UsersUnilever");

            migrationBuilder.RenameColumn(
                name: "RoleUnileverId",
                table: "RoleUnilevers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RoleUnileverId",
                table: "RoleAccesses",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleAccesses_RoleUnileverId",
                table: "RoleAccesses",
                newName: "IX_RoleAccesses_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleAccesses_RoleUnilevers_RoleId",
                table: "RoleAccesses",
                column: "RoleId",
                principalTable: "RoleUnilevers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
