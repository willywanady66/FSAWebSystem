using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ProposeAdditional",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Rephase",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ApprovalId",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "WeeklyBucketId",
                table: "ProposalDetails");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualProposeAdditional",
                table: "ProposalDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "BannerPlantId",
                table: "ProposalDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PlantContribution",
                table: "ProposalDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ProposalId",
                table: "Approvals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ApprovalDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Rephase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlantContribution = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalDetail_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProposalDetails_BannerPlantId",
                table: "ProposalDetails",
                column: "BannerPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ProposalId",
                table: "Approvals",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDetail_ApprovalId",
                table: "ApprovalDetail",
                column: "ApprovalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Proposals_ProposalId",
                table: "Approvals",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalDetails_BannerPlants_BannerPlantId",
                table: "ProposalDetails",
                column: "BannerPlantId",
                principalTable: "BannerPlants",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Proposals_ProposalId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalDetails_BannerPlants_BannerPlantId",
                table: "ProposalDetails");

            migrationBuilder.DropTable(
                name: "ApprovalDetail");

            migrationBuilder.DropIndex(
                name: "IX_ProposalDetails_BannerPlantId",
                table: "ProposalDetails");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_ProposalId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ActualProposeAdditional",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "BannerPlantId",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "PlantContribution",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "ProposalId",
                table: "Approvals");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalId",
                table: "Proposals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "ProposeAdditional",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Rephase",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalId",
                table: "ProposalDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WeeklyBucketId",
                table: "ProposalDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
