using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerManagementPlatform.Migrations
{
    public partial class AddAppointmentsAndObservations_UpdateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_AspNetUsers_TechnicianId",
                table: "Budgets");

            migrationBuilder.DropTable(
                name: "BudgetProduct");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_OrderId",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_TechnicianId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Appointment",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Creation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Budgets");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Products",
                newName: "BasePrice");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Execution",
                table: "Orders",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Orders",
                newName: "ClientDescription");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Clients",
                newName: "Tin");

            migrationBuilder.AddColumn<int>(
                name: "TaxRate",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(299)", maxLength: 299, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_OrderId",
                table: "Budgets",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_OrderId",
                table: "Appointments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TechnicianId",
                table: "Appointments",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_BudgetId",
                table: "Items",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ProductId",
                table: "Items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_OrderId",
                table: "Observations",
                column: "OrderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_OrderId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "Products",
                newName: "Tax");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Orders",
                newName: "Execution");

            migrationBuilder.RenameColumn(
                name: "ClientDescription",
                table: "Orders",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Tin",
                table: "Clients",
                newName: "Tax");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Appointment",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Creation",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Budgets",
                type: "nvarchar(299)",
                maxLength: 299,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Budgets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "Budgets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Budgets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "BudgetProduct",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetProduct", x => new { x.BudgetId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_BudgetProduct_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_OrderId",
                table: "Budgets",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_TechnicianId",
                table: "Budgets",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetProduct_ProductId",
                table: "BudgetProduct",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_AspNetUsers_TechnicianId",
                table: "Budgets",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
