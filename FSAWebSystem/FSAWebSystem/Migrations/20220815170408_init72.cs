using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class init72 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BannerId",
                table: "ProposalHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "ProposeAdditional",
                table: "ProposalHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "ProposalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Rephase",
                table: "ProposalHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "SKUId",
                table: "ProposalHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SubmittedAt",
                table: "ProposalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubmittedBy",
                table: "ProposalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "ProposalHistories");

            migrationBuilder.DropColumn(
                name: "ProposeAdditional",
                table: "ProposalHistories");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "ProposalHistories");

            migrationBuilder.DropColumn(
                name: "Rephase",
                table: "ProposalHistories");

            migrationBuilder.DropColumn(
                name: "SKUId",
                table: "ProposalHistories");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "ProposalHistories");

            migrationBuilder.DropColumn(
                name: "SubmittedBy",
                table: "ProposalHistories");
        }
    }
}
