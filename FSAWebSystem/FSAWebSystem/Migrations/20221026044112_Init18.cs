using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SkuId",
                table: "Proposals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_SkuId",
                table: "Proposals",
                column: "SkuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_SKUs_SkuId",
                table: "Proposals",
                column: "SkuId",
                principalTable: "SKUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_SKUs_SkuId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_SkuId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "SkuId",
                table: "Proposals");
        }
    }
}
