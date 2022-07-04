using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSAWebSystem.Migrations
{
    public partial class Init12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "UsersUnilever");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UsersUnilever",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
