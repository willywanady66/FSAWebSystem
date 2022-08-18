using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init65 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FSACalendarDetail_FSACalendarHeader_FSACalendarHeaderId",
                table: "FSACalendarDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FSACalendarHeader",
                table: "FSACalendarHeader");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FSACalendarDetail",
                table: "FSACalendarDetail");

            migrationBuilder.RenameTable(
                name: "FSACalendarHeader",
                newName: "FSACalendarHeaders");

            migrationBuilder.RenameTable(
                name: "FSACalendarDetail",
                newName: "FSACalendarDetails");

            migrationBuilder.RenameIndex(
                name: "IX_FSACalendarDetail_FSACalendarHeaderId",
                table: "FSACalendarDetails",
                newName: "IX_FSACalendarDetails_FSACalendarHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FSACalendarHeaders",
                table: "FSACalendarHeaders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FSACalendarDetails",
                table: "FSACalendarDetails",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ULICalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ULICalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ULICalendarDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ULICalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ULICalendarDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                        column: x => x.ULICalendarId,
                        principalTable: "ULICalendars",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ULICalendarDetails_ULICalendarId",
                table: "ULICalendarDetails",
                column: "ULICalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_FSACalendarDetails_FSACalendarHeaders_FSACalendarHeaderId",
                table: "FSACalendarDetails",
                column: "FSACalendarHeaderId",
                principalTable: "FSACalendarHeaders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FSACalendarDetails_FSACalendarHeaders_FSACalendarHeaderId",
                table: "FSACalendarDetails");

            migrationBuilder.DropTable(
                name: "ULICalendarDetails");

            migrationBuilder.DropTable(
                name: "ULICalendars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FSACalendarHeaders",
                table: "FSACalendarHeaders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FSACalendarDetails",
                table: "FSACalendarDetails");

            migrationBuilder.RenameTable(
                name: "FSACalendarHeaders",
                newName: "FSACalendarHeader");

            migrationBuilder.RenameTable(
                name: "FSACalendarDetails",
                newName: "FSACalendarDetail");

            migrationBuilder.RenameIndex(
                name: "IX_FSACalendarDetails_FSACalendarHeaderId",
                table: "FSACalendarDetail",
                newName: "IX_FSACalendarDetail_FSACalendarHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FSACalendarHeader",
                table: "FSACalendarHeader",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FSACalendarDetail",
                table: "FSACalendarDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FSACalendarDetail_FSACalendarHeader_FSACalendarHeaderId",
                table: "FSACalendarDetail",
                column: "FSACalendarHeaderId",
                principalTable: "FSACalendarHeader",
                principalColumn: "Id");
        }
    }
}
