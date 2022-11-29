using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RatingRate",
                table: "WeeklyBuckets",
                newName: "RunningRate");

            migrationBuilder.RenameColumn(
                name: "RatingRate",
                table: "MonthlyBuckets",
                newName: "RunningRate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RunningRate",
                table: "WeeklyBuckets",
                newName: "RatingRate");

            migrationBuilder.RenameColumn(
                name: "RunningRate",
                table: "MonthlyBuckets",
                newName: "RatingRate");
        }
    }
}
