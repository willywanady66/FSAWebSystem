using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Andromedas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UUStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ITThisWeek = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RRACT13Wk = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeekCover = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Andromedas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BottomPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvgNormalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvgBottomPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvgActualPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinActualPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remaks = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottomPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ITrusts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCMap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SumIntransit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SumStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SumFinalRpp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITrusts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Andromedas");

            migrationBuilder.DropTable(
                name: "BottomPrices");

            migrationBuilder.DropTable(
                name: "ITrusts");
        }
    }
}
