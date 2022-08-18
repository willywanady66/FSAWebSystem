using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
