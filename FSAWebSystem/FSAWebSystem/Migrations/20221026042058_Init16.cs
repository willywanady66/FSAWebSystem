using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalDetail");

            migrationBuilder.RenameColumn(
                name: "BannerId",
                table: "WeeklyBuckets",
                newName: "BannerPlantId");

            migrationBuilder.RenameColumn(
                name: "BannerId",
                table: "WeeklyBucketHistories",
                newName: "BannerPlantId");

            migrationBuilder.RenameColumn(
                name: "WeeklyBucketId",
                table: "Proposals",
                newName: "BannerId");

            migrationBuilder.RenameColumn(
                name: "BannerId",
                table: "ProposalHistories",
                newName: "BannerPlantId");

            migrationBuilder.RenameColumn(
                name: "BannerId",
                table: "MonthlyBuckets",
                newName: "BannerPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyBuckets_BannerPlantId",
                table: "WeeklyBuckets",
                column: "BannerPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_BannerId",
                table: "Proposals",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBuckets_BannerPlantId",
                table: "MonthlyBuckets",
                column: "BannerPlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyBuckets_BannerPlants_BannerPlantId",
                table: "MonthlyBuckets",
                column: "BannerPlantId",
                principalTable: "BannerPlants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Banners_BannerId",
                table: "Proposals",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyBuckets_BannerPlants_BannerPlantId",
                table: "WeeklyBuckets",
                column: "BannerPlantId",
                principalTable: "BannerPlants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyBuckets_BannerPlants_BannerPlantId",
                table: "MonthlyBuckets");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Banners_BannerId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyBuckets_BannerPlants_BannerPlantId",
                table: "WeeklyBuckets");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyBuckets_BannerPlantId",
                table: "WeeklyBuckets");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_BannerId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyBuckets_BannerPlantId",
                table: "MonthlyBuckets");

            migrationBuilder.RenameColumn(
                name: "BannerPlantId",
                table: "WeeklyBuckets",
                newName: "BannerId");

            migrationBuilder.RenameColumn(
                name: "BannerPlantId",
                table: "WeeklyBucketHistories",
                newName: "BannerId");

            migrationBuilder.RenameColumn(
                name: "BannerId",
                table: "Proposals",
                newName: "WeeklyBucketId");

            migrationBuilder.RenameColumn(
                name: "BannerPlantId",
                table: "ProposalHistories",
                newName: "BannerId");

            migrationBuilder.RenameColumn(
                name: "BannerPlantId",
                table: "MonthlyBuckets",
                newName: "BannerId");

            migrationBuilder.CreateTable(
                name: "ApprovalDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposeAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rephase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeeklyBucketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "IX_ApprovalDetail_ApprovalId",
                table: "ApprovalDetail",
                column: "ApprovalId");
        }
    }
}
