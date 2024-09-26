using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesOrderField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesOrder",
                table: "OrderData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesOrder",
                table: "OrderData");
        }
    }
}
