using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init43 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProposeAddional",
                table: "Proposals",
                newName: "ProposeAdditional");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Week",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Week",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "ProposeAdditional",
                table: "Proposals",
                newName: "ProposeAddional");
        }
    }
}
