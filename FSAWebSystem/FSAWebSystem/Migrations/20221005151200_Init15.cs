using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Trade",
                table: "BannerPlants");

            migrationBuilder.AddColumn<string>(
                name: "Trade",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Trade",
                table: "Banners");

            migrationBuilder.AddColumn<string>(
                name: "Trade",
                table: "BannerPlants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
