using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Migrations
{
    /// <inheritdoc />
    public partial class DelStringCustOnOrderData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer",
                table: "OrderData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "OrderData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
