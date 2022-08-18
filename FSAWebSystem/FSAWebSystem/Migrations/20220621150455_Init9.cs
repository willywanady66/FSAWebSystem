using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "UsersUnilever");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "SKUs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SKUs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "BannerUserUnilever",
                columns: table => new
                {
                    BannerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserUnileversId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerUserUnilever", x => new { x.BannerId, x.UserUnileversId });
                    table.ForeignKey(
                        name: "FK_BannerUserUnilever_Banners_BannerId",
                        column: x => x.BannerId,
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

            migrationBuilder.CreateTable(
                name: "RoleUnilevers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUnilevers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Menu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubMenu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessActivity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleAccesses_RoleUnilevers_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleUnilevers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RoleUnilevers",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { new Guid("7c90cac7-05e0-4ad3-b117-bf2107c9ffdf"), "Administrator" },
                    { new Guid("7d16658d-47ea-40a8-8e3d-900e1f263304"), "Requestor" },
                    { new Guid("c270be53-53e3-4330-8b7d-4a72a1277fec"), "Approver" },
                    { new Guid("d0f9d21f-4418-4187-8cee-6d854da377c0"), "Support" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SKUs_ProductCategoryId",
                table: "SKUs",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerUserUnilever_UserUnileversId",
                table: "BannerUserUnilever",
                column: "UserUnileversId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_RoleId",
                table: "RoleAccesses",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_SKUs_ProductCategories_ProductCategoryId",
                table: "SKUs",
                column: "ProductCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SKUs_ProductCategories_ProductCategoryId",
                table: "SKUs");

            migrationBuilder.DropTable(
                name: "BannerUserUnilever");

            migrationBuilder.DropTable(
                name: "RoleAccesses");

            migrationBuilder.DropTable(
                name: "RoleUnilevers");

            migrationBuilder.DropIndex(
                name: "IX_SKUs_ProductCategoryId",
                table: "SKUs");

            migrationBuilder.AddColumn<Guid>(
                name: "BannerId",
                table: "UsersUnilever",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "SKUs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SKUs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { new Guid("572224d3-812e-4c84-8389-c1c24437fc8c"), "Support" },
                    { new Guid("65df833d-e8a6-4c81-be7a-f8fdc32ae86a"), "Requestor" },
                    { new Guid("8b5c2c86-42bd-44fc-b94e-6c7df43d5267"), "Administrator" },
                    { new Guid("f25442ed-fcf2-4218-8f1d-8c4fc8fd1cb1"), "Approver" }
                });
        }
    }
}
