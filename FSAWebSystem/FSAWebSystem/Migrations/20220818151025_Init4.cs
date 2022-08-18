using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProposalId",
                table: "ProposalHistories",
                newName: "SKUId");

            migrationBuilder.AddColumn<Guid>(
                name: "BannerId",
                table: "ProposalHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "ProposalHistories");

            migrationBuilder.RenameColumn(
                name: "SKUId",
                table: "ProposalHistories",
                newName: "ProposalId");
        }
    }
}
