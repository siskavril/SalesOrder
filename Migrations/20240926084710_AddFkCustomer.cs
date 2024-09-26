using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Migrations
{
    /// <inheritdoc />
    public partial class AddFkCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderData_IdCustomer",
                table: "OrderData",
                column: "IdCustomer");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderData_Customer_IdCustomer",
                table: "OrderData",
                column: "IdCustomer",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderData_Customer_IdCustomer",
                table: "OrderData");

            migrationBuilder.DropIndex(
                name: "IX_OrderData_IdCustomer",
                table: "OrderData");
        }
    }
}
