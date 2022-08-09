using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init59 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Approvals");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalId",
                table: "UsersUnilever",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProposalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    ProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersUnilever_ApprovalId",
                table: "UsersUnilever",
                column: "ApprovalId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersUnilever_Approvals_ApprovalId",
                table: "UsersUnilever",
                column: "ApprovalId",
                principalTable: "Approvals",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersUnilever_Approvals_ApprovalId",
                table: "UsersUnilever");

            migrationBuilder.DropTable(
                name: "ProposalHistories");

            migrationBuilder.DropIndex(
                name: "IX_UsersUnilever_ApprovalId",
                table: "UsersUnilever");

            migrationBuilder.DropColumn(
                name: "ApprovalId",
                table: "UsersUnilever");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Approvals",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
