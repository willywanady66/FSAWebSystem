using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProposeAdditional",
                table: "ProposalDetails");

            migrationBuilder.RenameColumn(
                name: "Rephase",
                table: "ProposalDetails",
                newName: "ActualRephase");

            migrationBuilder.AddColumn<decimal>(
                name: "ProposeAdditional",
                table: "Proposals",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Rephase",
                table: "Proposals",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProposeAdditional",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Rephase",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "ActualRephase",
                table: "ProposalDetails",
                newName: "Rephase");

            migrationBuilder.AddColumn<decimal>(
                name: "ProposeAdditional",
                table: "ProposalDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
