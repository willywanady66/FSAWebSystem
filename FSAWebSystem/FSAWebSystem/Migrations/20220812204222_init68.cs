using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init68 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "ULICalendarId",
                table: "ULICalendarDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "ULICalendarDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ULICalendarDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails",
                column: "ULICalendarId",
                principalTable: "ULICalendars",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "ULICalendarDetails");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "ULICalendarDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "ULICalendarId",
                table: "ULICalendarDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ULICalendarDetails_ULICalendars_ULICalendarId",
                table: "ULICalendarDetails",
                column: "ULICalendarId",
                principalTable: "ULICalendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
