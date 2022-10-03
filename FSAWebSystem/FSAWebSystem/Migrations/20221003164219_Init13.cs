using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerPlants_Banner_BannerId",
                table: "BannerPlants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banner",
                table: "Banner");

            migrationBuilder.RenameTable(
                name: "Banner",
                newName: "Banners");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banners",
                table: "Banners",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerPlants_Banners_BannerId",
                table: "BannerPlants",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerPlants_Banners_BannerId",
                table: "BannerPlants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banners",
                table: "Banners");

            migrationBuilder.RenameTable(
                name: "Banners",
                newName: "Banner");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banner",
                table: "Banner",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerPlants_Banner_BannerId",
                table: "BannerPlants",
                column: "BannerId",
                principalTable: "Banner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
