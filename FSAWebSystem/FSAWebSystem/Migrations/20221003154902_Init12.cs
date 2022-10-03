using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannerUserUnilever");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banners",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "CDM",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "FSADocumentId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "KAM",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "PlantCode",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "PlantName",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Trade",
                table: "Banners");

            migrationBuilder.RenameTable(
                name: "Banners",
                newName: "Banner");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banner",
                table: "Banner",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Plant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BannerPlants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Trade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CDM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KAM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FSADocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerPlants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannerPlants_Banner_BannerId",
                        column: x => x.BannerId,
                        principalTable: "Banner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannerPlants_Plant_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BannerPlantUserUnilever",
                columns: table => new
                {
                    BannerPlantsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerPlantUserUnilever", x => new { x.BannerPlantsId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_BannerPlantUserUnilever_BannerPlants_BannerPlantsId",
                        column: x => x.BannerPlantsId,
                        principalTable: "BannerPlants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannerPlantUserUnilever_UsersUnilever_UserUnileversId",
                        column: x => x.UserUnileversId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BannerPlants_BannerId",
                table: "BannerPlants",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerPlants_PlantId",
                table: "BannerPlants",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerPlantUserUnilever_UserUnileversId",
                table: "BannerPlantUserUnilever",
                column: "UserUnileversId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannerPlantUserUnilever");

            migrationBuilder.DropTable(
                name: "BannerPlants");

            migrationBuilder.DropTable(
                name: "Plant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banner",
                table: "Banner");

            migrationBuilder.RenameTable(
                name: "Banner",
                newName: "Banners");

            migrationBuilder.AddColumn<string>(
                name: "CDM",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Banners",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FSADocumentId",
                table: "Banners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Banners",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "KAM",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Banners",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlantCode",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlantName",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Trade",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banners",
                table: "Banners",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BannerUserUnilever",
                columns: table => new
                {
                    BannersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerUserUnilever", x => new { x.BannersId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_BannerUserUnilever_Banners_BannersId",
                        column: x => x.BannersId,
                        principalTable: "Banners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannerUserUnilever_UsersUnilever_UserUnileversId",
                        column: x => x.UserUnileversId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BannerUserUnilever_UserUnileversId",
                table: "BannerUserUnilever",
                column: "UserUnileversId");
        }
    }
}
