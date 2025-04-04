using Microsoft.EntityFrameworkCore.Migrations;

namespace AppCalisto.Migrations
{
    public partial class UpdateUsers_AddCompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Companies",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Companies",
                table: "AspNetUsers");
        }
    }
}
