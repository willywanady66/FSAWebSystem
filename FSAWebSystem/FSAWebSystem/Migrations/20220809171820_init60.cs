using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init60 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersUnilever_Approvals_ApprovalId",
                table: "UsersUnilever");

            migrationBuilder.DropIndex(
                name: "IX_UsersUnilever_ApprovalId",
                table: "UsersUnilever");

            migrationBuilder.DropColumn(
                name: "ApprovalId",
                table: "UsersUnilever");

            migrationBuilder.CreateTable(
                name: "ApprovalUserUnilever",
                columns: table => new
                {
                    ApprovalsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalUserUnilever", x => new { x.ApprovalsId, x.ApprovedById });
                    table.ForeignKey(
                        name: "FK_ApprovalUserUnilever_Approvals_ApprovalsId",
                        column: x => x.ApprovalsId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalUserUnilever_UsersUnilever_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalUserUnilever_ApprovedById",
                table: "ApprovalUserUnilever",
                column: "ApprovedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalUserUnilever");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalId",
                table: "UsersUnilever",
                type: "uniqueidentifier",
                nullable: true);

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
    }
}
