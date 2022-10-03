using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerPlants_Plant_PlantId",
                table: "BannerPlants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plant",
                table: "Plant");

            migrationBuilder.RenameTable(
                name: "Plant",
                newName: "Plants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plants",
                table: "Plants",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerPlants_Plants_PlantId",
                table: "BannerPlants",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerPlants_Plants_PlantId",
                table: "BannerPlants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plants",
                table: "Plants");

            migrationBuilder.RenameTable(
                name: "Plants",
                newName: "Plant");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plant",
                table: "Plant",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerPlants_Plant_PlantId",
                table: "BannerPlants",
                column: "PlantId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
