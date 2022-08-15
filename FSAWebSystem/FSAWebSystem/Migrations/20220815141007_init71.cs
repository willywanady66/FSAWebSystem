using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init71 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails");

            migrationBuilder.DropTable(
                name: "ApprovalUserUnilever");

            migrationBuilder.AlterColumn<Guid>(
                name: "ULICalendarId",
                table: "ULICalendarDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Approvals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Approvals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUnileverId",
                table: "Approvals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_UserUnileverId",
                table: "Approvals",
                column: "UserUnileverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_UsersUnilever_UserUnileverId",
                table: "Approvals",
                column: "UserUnileverId",
                principalTable: "UsersUnilever",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails",
                column: "ULICalendarId",
                principalTable: "ULICalendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_UsersUnilever_UserUnileverId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_UserUnileverId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "UserUnileverId",
                table: "Approvals");

            migrationBuilder.AlterColumn<Guid>(
                name: "ULICalendarId",
                table: "ULICalendarDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails",
                column: "ULICalendarId",
                principalTable: "ULICalendars",
                principalColumn: "Id");
        }
    }
}
