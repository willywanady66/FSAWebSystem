using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategoryUserUnilever",
                columns: table => new
                {
                    ProductCategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategoryUserUnilever", x => new { x.ProductCategoriesId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_ProductCategoryUserUnilever_ProductCategories_ProductCategoriesId",
                        column: x => x.ProductCategoriesId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategoryUserUnilever_UsersUnilever_UserUnileversId",
                        column: x => x.UserUnileversId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SKUUserUnilever",
                columns: table => new
                {
                    SKUsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKUUserUnilever", x => new { x.SKUsId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_SKUUserUnilever_SKUs_SKUsId",
                        column: x => x.SKUsId,
                        principalTable: "SKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SKUUserUnilever_UsersUnilever_UserUnileversId",
                        column: x => x.UserUnileversId,
                        principalTable: "UsersUnilever",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryUserUnilever_UserUnileversId",
                table: "ProductCategoryUserUnilever",
                column: "UserUnileversId");

            migrationBuilder.CreateIndex(
                name: "IX_SKUUserUnilever_UserUnileversId",
                table: "SKUUserUnilever",
                column: "UserUnileversId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategoryUserUnilever");

            migrationBuilder.DropTable(
                name: "SKUUserUnilever");
        }
    }
}
