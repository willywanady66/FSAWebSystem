using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerUserUnilever_Banners_BannerId",
                table: "BannerUserUnilever");

            migrationBuilder.DeleteData(
                table: "RoleUnilevers",
                keyColumn: "Id",
                keyValue: new Guid("7c90cac7-05e0-4ad3-b117-bf2107c9ffdf"));

            migrationBuilder.DeleteData(
                table: "RoleUnilevers",
                keyColumn: "Id",
                keyValue: new Guid("7d16658d-47ea-40a8-8e3d-900e1f263304"));

            migrationBuilder.DeleteData(
                table: "RoleUnilevers",
                keyColumn: "Id",
                keyValue: new Guid("c270be53-53e3-4330-8b7d-4a72a1277fec"));

            migrationBuilder.DeleteData(
                table: "RoleUnilevers",
                keyColumn: "Id",
                keyValue: new Guid("d0f9d21f-4418-4187-8cee-6d854da377c0"));

            migrationBuilder.RenameColumn(
                name: "BannerId",
                table: "BannerUserUnilever",
                newName: "BannersId");

            migrationBuilder.AddForeignKey(
                name: "FK_BannerUserUnilever_Banners_BannersId",
                table: "BannerUserUnilever",
                column: "BannersId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannerUserUnilever_Banners_BannersId",
                table: "BannerUserUnilever");

            migrationBuilder.RenameColumn(
                name: "BannersId",
                table: "BannerUserUnilever",
                newName: "BannerId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_BannerUserUnilever_Banners_BannerId",
                table: "BannerUserUnilever",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
