using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeeklyBuckets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BannerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonthlyBucket = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RatingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BucketWeek1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BucketWeek2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BucketWeek3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BucketWeek4 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BucketWeek5 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValidBJ = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemFSA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DispatchConsume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyBuckets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyBuckets");
        }
    }
}
