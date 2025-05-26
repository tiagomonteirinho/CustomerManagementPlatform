using Microsoft.EntityFrameworkCore.Migrations;

namespace AppCalisto.Migrations
{
    public partial class AddObservationImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObservationImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObservationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationImages_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationImages_ObservationId",
                table: "ObservationImages",
                column: "ObservationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservationImages");
        }
    }
}
